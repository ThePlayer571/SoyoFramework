using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.Framework.Runtime.UsefulTools;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core
{
    public abstract class ProcedureManager<TProcedureId, TTagId> : IProcedureManager<TProcedureId, TTagId>
    {
        #region 属性

        public TProcedureId CurrentProcedure { get; private set; }
        public ProcedureCheckMode CheckMode { get; set; } = ProcedureCheckMode.Warning;

        #endregion

        #region 字段

        // 事件存储
        private readonly Dictionary<(TProcedureId, ProcedureChangeStage), EasyEvent<ProcedureChangeInfo>>
            _procedureCallbacks = new();

        // 防中断设计：允许在上个流程结束前切换流程
        private readonly Queue<(TProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras)>
            _procedureChangeQueue = new();

        private bool _isChangingProcedure;

        // 变量
        private ProcedureChangeInfo.ProcedureChangeParas _lastProcedureChangeParas;
        private readonly Queue<UniTask> _awaitTasks = new();

        // 配置信息
        private ProcedureConfig<TProcedureId, TTagId> _config;

        #endregion

        #region 构造函数

        protected ProcedureManager(ProcedureConfig<TProcedureId, TTagId> config)
        {
            _config = config;
            CurrentProcedure = config.InitialProcedure;
        }

        #endregion

        #region 接口实现 - 流程切换

        public async UniTask ChangeProcedure(TProcedureId procedureId, params (string, object)[] paras)
        {
            await ChangeProcedure(procedureId, new ProcedureChangeInfo.ProcedureChangeParas(paras));
        }

        public async UniTask ChangeProcedure(TProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras)
        {
            var leaveFrom = CurrentProcedure;
            var changeTo = procedureId;

            // 检查前置流程
            if (!CheckAllowedPreviousProcedure(leaveFrom, changeTo))
            {
                switch (CheckMode)
                {
                    case ProcedureCheckMode.ErrorAndStop:
                        $"进行了不允许的流程切换，已阻断：{leaveFrom} -> {changeTo}".LogError();
                        return;
                    case ProcedureCheckMode.Warning:
                        $" 进行了不允许的流程切换：{leaveFrom} -> {changeTo}".LogWarning();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

        public IUnRegister RegisterProcedure(TProcedureId procedureId, ProcedureChangeStage stage,
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

        public bool HasTag(TProcedureId procedureId, TTagId tag)
        {
            if (_config.MetaDatas.TryGetValue(procedureId, out var metaData))
            {
                return metaData.Tags.Contains(tag);
            }

            "未找到ProcedureId {procedureId} 的配置数据，无法检查标签。".LogError();
            return false;
        }

        public IReadOnlyCollection<TTagId> GetTags(TProcedureId procedureId)
        {
            if (_config.MetaDatas.TryGetValue(procedureId, out var metaData))
            {
                return metaData.Tags;
            }

            "未找到ProcedureId {procedureId} 的配置数据，无法获取标签。".LogError();
            return Array.Empty<TTagId>();
        }

        public bool CurrentHasTag(TTagId tag)
        {
            return HasTag(CurrentProcedure, tag);
        }

        public IReadOnlyCollection<TTagId> GetCurrentTags()
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
        /// <returns></returns>
        private bool CheckAllowedPreviousProcedure(TProcedureId from, TProcedureId to)
        {
            if (!_config.MetaDatas.TryGetValue(to, out var mataData))
            {
                $"未找到ProcedureId {to} 的配置数据".LogError();
                return false;
            }

            var allowedPrevious = mataData.AllowingPreviousProcedures;

            return allowedPrevious.Contains(from);
        }

        private void ExecuteCallbacks((TProcedureId, ProcedureChangeStage) trigger,
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