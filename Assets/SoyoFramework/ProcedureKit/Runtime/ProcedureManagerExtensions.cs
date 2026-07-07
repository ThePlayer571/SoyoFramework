using System.Threading;
using Cysharp.Threading.Tasks;
using SoyoFramework.ProcedureKit.Runtime.Core;
using SoyoFramework.ProcedureKit.Runtime.DataClasses;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework.ProcedureKit.Runtime
{
    public static class ProcedureManagerExtensions
    {
        public static void AddAwait<TProcedureId, TTag>(
            this UniTask task,
            IProcedureManager<TProcedureId, TTag> procedureManager)
        {
            procedureManager.AddAwait(task);
        }

        /// <summary>
        /// 返回一个UniTask，在指定阶段流程切换到指定流程时完成
        /// </summary>
        public static UniTask WaitUntilProcedure<TProcedureId, TTagId>(
            this IReadonlyProcedureManager<TProcedureId, TTagId> procedureManager,
            TProcedureId targetProcedure,
            ProcedureChangeStage targetStage,
            CancellationToken cancellationToken = default)
        {
            var tcs = new UniTaskCompletionSource();

            // 如果已经在目标流程和阶段，直接完成
            if (targetStage == ProcedureChangeStage.EnterEarly && procedureManager.CurrentProcedure != null &&
                procedureManager.CurrentProcedure.Equals(targetProcedure))
            {
                tcs.TrySetResult();
                return tcs.Task;
            }

            IUnRegister unRegister = null;
            unRegister = procedureManager.OnProcedureChange.Register((procedureId, changeStage) =>
            {
                if (changeStage.Equals(targetStage) && procedureId.Equals(targetProcedure))
                {
                    unRegister.UnRegister();
                    tcs.TrySetResult();
                }
            });

            // 支持取消
            if (cancellationToken != default)
            {
                cancellationToken.Register(() =>
                {
                    unRegister?.UnRegister();
                    tcs.TrySetCanceled();
                });
            }

            return tcs.Task;
        }
    }
}