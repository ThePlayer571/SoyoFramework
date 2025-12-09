using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace SoyoFramework.Framework.Runtime.ProcedureKit
{
    public static class ProcedureManagerExtensions
    {
        public static void AddAwait(this UniTask task, IProcedureManager procedureManager)
        {
            procedureManager.AddAwait(task);
        }

        public static void AddAwait<T>(this UniTask<T> task, IProcedureManager procedureManager)
        {
            procedureManager.AddAwait(task.AsUniTask());
        }
    }
}