using System;
using Cysharp.Threading.Tasks;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime
{
    public static class ProcedureManagerExtensions
    {
        public static void AddAwait<TProcedureId, TTagId>
            (this UniTask task, IProcedureManager<TProcedureId, TTagId> procedureManager)
        {
            procedureManager.AddAwait(task);
        }

        public static void AddAwait<T, TProcedureId, TTagId>
            (this UniTask<T> task, IProcedureManager<TProcedureId, TTagId> procedureManager)
        {
            procedureManager.AddAwait(task.AsUniTask());
        }
    }
}