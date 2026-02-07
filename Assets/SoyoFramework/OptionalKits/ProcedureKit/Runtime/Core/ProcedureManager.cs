using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core
{
    public abstract class ProcedureManager<TProcedureId, TTagId> : IProcedureManager<TProcedureId, TTagId>
    {
        #region 属性

        public TProcedureId CurrentProcedure { get; private set; }
        public ProcedureCheckMode CheckMode { get; set; } = ProcedureCheckMode.Warning;
        public bool IsChangingProcedure => _isChangingProcedure;

        #endregion

        #region 字段

        // callback event storage
        private readonly Dictionary<(TProcedureId, ProcedureChangeStage), Action<ProcedureChangeInfo>>
            _procedureCallbacks = new();

        // 用于防止中断和支持排队请求
        private class ProcedureChangeRequest
        {
            public TProcedureId ProcedureId;
            public ProcedureChangeInfo.ProcedureChangeParas Paras;
            public UniTaskCompletionSource CompletionSource;
        }

        private readonly Queue<ProcedureChangeRequest> _procedureChangeQueue = new();
        private bool _isChangingProcedure;

        // 变量
        private ProcedureChangeInfo.ProcedureChangeParas _lastProcedureChangeParas;
        private readonly Queue<UniTask> _awaitTasks = new();
        private readonly EasyEvent<TProcedureId, ProcedureChangeStage> _onProcedureChange = new();

        // 配置
        private ProcedureConfig<TProcedureId, TTagId> _config;

        // Logger
        protected readonly ILog Logger = new PrefixLogger("[ProcedureKit]");

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
            var tcs = new UniTaskCompletionSource();

            // 入队
            _procedureChangeQueue.Enqueue(new ProcedureChangeRequest
            {
                ProcedureId = procedureId,
                Paras = paras,
                CompletionSource = tcs
            });

            // 如果当前已有流程在切换则等待
            if (_isChangingProcedure)
            {
                await tcs.Task;
                return;
            }

            _isChangingProcedure = true;

            // 按队列顺序依次处理所有请求
            while (_procedureChangeQueue.Count > 0)
            {
                var request = _procedureChangeQueue.Dequeue();
                await ProcessProcedureChange(request.ProcedureId, request.Paras);
                request.CompletionSource.TrySetResult();
            }

            _isChangingProcedure = false;
        }

        // 只处理实际切换，不做队列和排队
        private async UniTask ProcessProcedureChange(TProcedureId procedureId,
            ProcedureChangeInfo.ProcedureChangeParas paras)
        {
            var leaveFrom = CurrentProcedure;
            var changeTo = procedureId;

            // 检查前置流程
            if (!CheckAllowedPreviousProcedure(leaveFrom, changeTo))
            {
                switch (CheckMode)
                {
                    case ProcedureCheckMode.ErrorAndStop:
                        $"进行了不允许的流程切换，已阻断：{leaveFrom} -> {changeTo}".LogError(Logger);
                        return;
                    case ProcedureCheckMode.Warning:
                        $"进行了不允许的流程切换：{leaveFrom} -> {changeTo}".LogWarning(Logger);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // 离开阶段
            _onProcedureChange.Trigger(leaveFrom, ProcedureChangeStage.BeforeLeave);
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.BeforeLeave), _lastProcedureChangeParas);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(leaveFrom, ProcedureChangeStage.LeaveEarly);
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.LeaveEarly), _lastProcedureChangeParas);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(leaveFrom, ProcedureChangeStage.LeaveNormal);
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.LeaveNormal), _lastProcedureChangeParas);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(leaveFrom, ProcedureChangeStage.LeaveLate);
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.LeaveLate), _lastProcedureChangeParas);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(leaveFrom, ProcedureChangeStage.AfterLeave);
            ExecuteCallbacks((leaveFrom, ProcedureChangeStage.AfterLeave), _lastProcedureChangeParas);
            await WaitForAllTasks();

            // 切换
            CurrentProcedure = changeTo;

            // 进入阶段
            _onProcedureChange.Trigger(changeTo, ProcedureChangeStage.BeforeEnter);
            ExecuteCallbacks((changeTo, ProcedureChangeStage.BeforeEnter), paras);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(changeTo, ProcedureChangeStage.EnterEarly);
            ExecuteCallbacks((changeTo, ProcedureChangeStage.EnterEarly), paras);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(changeTo, ProcedureChangeStage.EnterNormal);
            ExecuteCallbacks((changeTo, ProcedureChangeStage.EnterNormal), paras);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(changeTo, ProcedureChangeStage.EnterLate);
            ExecuteCallbacks((changeTo, ProcedureChangeStage.EnterLate), paras);
            await WaitForAllTasks();
            _onProcedureChange.Trigger(changeTo, ProcedureChangeStage.AfterEnter);
            ExecuteCallbacks((changeTo, ProcedureChangeStage.AfterEnter), paras);
            await WaitForAllTasks();

            _lastProcedureChangeParas = paras;
        }

        public IUnRegister Register(TProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback)
        {
            var trigger = (procedureId, stage);

            _procedureCallbacks.TryAdd(trigger, null);
            _procedureCallbacks[trigger] += callback;

            // 返回IUnRegister句柄，用于后期移除回调
            return new CustomUnRegister(() => { _procedureCallbacks[trigger] -= callback; });
        }

        public EasyEvent<TProcedureId, ProcedureChangeStage> OnProcedureChange => _onProcedureChange;

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

            $"未找到ProcedureId {procedureId} 的配置数据，无法检查标签。".LogError(Logger);
            return false;
        }

        public IReadOnlyCollection<TTagId> GetTags(TProcedureId procedureId)
        {
            if (_config.MetaDatas.TryGetValue(procedureId, out var metaData))
            {
                return metaData.Tags;
            }

            $"未找到ProcedureId {procedureId} 的配置数据，无法获取标签。".LogError(Logger);
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
                $"未找到ProcedureId {to} 的配置数据".LogError(Logger);
                return false;
            }

            var allowedPrevious = mataData.AllowingPreviousProcedures;
            return allowedPrevious.Contains(from);
        }

        private void ExecuteCallbacks((TProcedureId, ProcedureChangeStage) trigger,
            ProcedureChangeInfo.ProcedureChangeParas changeParas)
        {
            // todo 这里需要把每个函数分别执行，并try catch，如果出错，仅仅是发出Log，而并非抛出错误
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
                    $"等待任务发生异常: {e}".LogError(Logger);
                }
            }
        }

        #endregion
    }
}