using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.CoreKits;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.Framework.Runtime.ProcedureKit.DataClasses;
using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;
using SoyoFramework.Scripts.ToolKits.PhaseKit;

namespace SoyoFramework.Framework.Runtime.ProcedureKit
{
    public interface IProcedureManager
    {
        ProcedureId CurrentProcedure { get; }
        ProcedureCheckMode CheckMode { get; set; }

        // 流程切换
        UniTask ChangeProcedure(ProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras);
        UniTask ChangeProcedure(ProcedureId procedureId, params (string, object)[] paras);

        // 流程订阅
        IUnRegister RegisterProcedure(ProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback);

        // 延迟切换
        void AddAwait(UniTask task);

        // 标签
        bool HasTag(ProcedureId procedureId, ProcedureTag tag);
        IReadOnlyList<ProcedureTag> GetTags(ProcedureId procedureId);
        bool CurrentHasTag(ProcedureTag tag);
        IReadOnlyList<ProcedureTag> GetCurrentTags();
    }

    // todo 改为internal
    public class ProcedureManager : IProcedureManager
    {
        #region 属性

        public ProcedureId CurrentProcedure { get; private set; }
        public ProcedureCheckMode CheckMode { get; set; } = ProcedureCheckMode.Warning;

        #endregion

        #region 字段

        // 事件存储
        private readonly Dictionary<(ProcedureId, ProcedureChangeStage), EasyEvent<ProcedureChangeInfo>>
            _procedureCallbacks = new();

        // 防中断设计：允许在上个流程结束前切换流程
        private readonly Queue<(ProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras)>
            _procedureChangeQueue = new();

        private bool _isChangingProcedure;

        // 变量
        private ProcedureChangeInfo.ProcedureChangeParas _lastProcedureChangeParas;
        private readonly Queue<UniTask> _awaitTasks = new();

        // 反射缓存
        private readonly Dictionary<ProcedureId, IReadOnlyList<ProcedureTag>> _tagsCache = new();
        private readonly Dictionary<ProcedureId, IReadOnlyList<ProcedureId>> _allowedPreviousCache = new();

        #endregion

        #region 构造函数

        public ProcedureManager()
        {
            CurrentProcedure = ProcedureId.Entrance;
            InitializeReflectionCache();
        }

        #endregion

        #region 初始化

        private void InitializeReflectionCache()
        {
            var procedureIdType = typeof(ProcedureId);
            var fields = procedureIdType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (!field.IsLiteral) continue;

                var procedureId = (ProcedureId)field.GetValue(null);

                // 缓存 Tags
                var tagsAttr = field.GetCustomAttribute<ProcedureTagsAttribute>();
                var tags = tagsAttr?.Tags ?? Array.Empty<ProcedureTag>();
                _tagsCache[procedureId] = tags;

                // 缓存 AllowedPreviousProcedures
                var allowedPrevAttr = field.GetCustomAttribute<AllowedPreviousProceduresAttribute>();
                var allowedPrevious = allowedPrevAttr?.AllowedPrevious ?? Array.Empty<ProcedureId>();
                _allowedPreviousCache[procedureId] = allowedPrevious;
            }
        }

        #endregion

        #region 接口实现 - 流程切换

        public async UniTask ChangeProcedure(ProcedureId procedureId, params (string, object)[] paras)
        {
            await ChangeProcedure(procedureId, new ProcedureChangeInfo.ProcedureChangeParas(paras));
        }

        public async UniTask ChangeProcedure(ProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras)
        {
            var leaveFrom = CurrentProcedure;
            var changeTo = procedureId;

            // 检查前置流程
            if (!CheckAllowedPreviousProcedure(leaveFrom, changeTo))
            {
                return; // ErrorAndStop 模式下阻断切换
            }

            // 防中断设计：若正在切换流程，则将请求入队列
            if (_isChangingProcedure)
            {
                _procedureChangeQueue.Enqueue((procedureId, paras));
                return;
            }

            // 开始切换
            _isChangingProcedure = true;

            // 离开上个流程
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.LeaveEarly), _lastProcedureChangeParas);
            await WaitForAllTasks();
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.LeaveNormal), _lastProcedureChangeParas);
            await WaitForAllTasks();
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.LeaveLate), _lastProcedureChangeParas);
            await WaitForAllTasks();

            // 进入下个流程
            CurrentProcedure = changeTo;
            ExecuteCallbacks((changeTo, ProcedureChangeStage.EnterEarly), paras);
            await WaitForAllTasks();
            ExecuteCallbacks((changeTo, ProcedureChangeStage.EnterNormal), paras);
            await WaitForAllTasks();
            ExecuteCallbacks((changeTo, ProcedureChangeStage.EnterLate), paras);
            await WaitForAllTasks();

            // 结束切换
            _lastProcedureChangeParas = paras;
            _isChangingProcedure = false;

            // 处理队列中的下个流程
            if (_procedureChangeQueue.Count > 0)
            {
                var (nextProcedure, nextParas) = _procedureChangeQueue.Dequeue();
                ChangeProcedure(nextProcedure, nextParas).Forget();
            }
        }

        public IUnRegister RegisterProcedure(ProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback)
        {
            var trigger = (procedureId, stage);
            if (!_procedureCallbacks.TryGetValue(trigger, out var easyEvent))
            {
                easyEvent = new EasyEvent<ProcedureChangeInfo>();
                _procedureCallbacks[trigger] = easyEvent;
            }

            return easyEvent.Register(callback);
        }

        public void AddAwait(UniTask task)
        {
            _awaitTasks.Enqueue(task);
        }

        #endregion

        #region 接口实现 - 标签

        public bool HasTag(ProcedureId procedureId, ProcedureTag tag)
        {
            if (_tagsCache.TryGetValue(procedureId, out var tags))
            {
                return tags.Contains(tag);
            }

            return false;
        }

        public IReadOnlyList<ProcedureTag> GetTags(ProcedureId procedureId)
        {
            if (_tagsCache.TryGetValue(procedureId, out var tags))
            {
                return tags;
            }

            return Array.Empty<ProcedureTag>();
        }

        public bool CurrentHasTag(ProcedureTag tag)
        {
            return HasTag(CurrentProcedure, tag);
        }

        public IReadOnlyList<ProcedureTag> GetCurrentTags()
        {
            return GetTags(CurrentProcedure);
        }

        #endregion

        #region 内部函数

        /// <summary>
        /// 检查前置流程是否允许
        /// </summary>
        /// <param name="from">当前流程</param>
        /// <param name="to">目标流程</param>
        /// <returns>true: 允许切换; false: 阻断切换 (仅在 ErrorAndStop 模式下)</returns>
        private bool CheckAllowedPreviousProcedure(ProcedureId from, ProcedureId to)
        {
            if (!_allowedPreviousCache.TryGetValue(to, out var allowedPrevious))
            {
                // 未配置 AllowedPreviousProcedures，默认允许所有
                return true;
            }

            // 如果 AllowedPrevious 为空数组，表示允许所有前置流程
            if (allowedPrevious.Count == 0)
            {
                return true;
            }

            // 检查当前流程是否在允许列表中
            if (allowedPrevious.Contains(from))
            {
                return true;
            }

            // 不允许的流程切换
            var message = $"[ProcedureKit] 进行了不允许的流程切换: {from} -> {to}";

            switch (CheckMode)
            {
                case ProcedureCheckMode.ErrorAndStop:
                    message.LogError();
                    return false;
                case ProcedureCheckMode.Warning:
                    message.LogWarning();
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecuteCallbacks((ProcedureId, ProcedureChangeStage) trigger,
            ProcedureChangeInfo.ProcedureChangeParas changeParas)
        {
            if (_procedureCallbacks.TryGetValue(trigger, out var easyEvent))
            {
                try
                {
                    easyEvent.Trigger(new ProcedureChangeInfo { Paras = changeParas });
                }
                catch (Exception e)
                {
                    e.LogError();
                }
            }
        }

        private async UniTask WaitForAllTasks()
        {
            while (_awaitTasks.Count > 0)
            {
                var task = _awaitTasks.Dequeue();
                try
                {
                    await task;
                }
                catch (Exception e)
                {
                    $"[ProcedureKit] 等待任务发生异常: {e}".LogError();
                }
            }
        }

        #endregion
    }
}