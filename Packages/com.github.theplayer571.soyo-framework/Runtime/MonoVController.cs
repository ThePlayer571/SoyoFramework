using UnityEngine;

namespace SoyoFramework
{
    public abstract class MonoVController : MonoBehaviour, IViewController
    {
        protected void SendCommand(ICommand command) => Architecture.Instance.SendCommand(command);

        protected TResult SendCommand<TResult>(ICommand<TResult> command)
            => Architecture.Instance.SendCommand(command);

        protected bool TrySendCommand(ICommand command, out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out canExecuteResult);

        protected bool TrySendCommand(ICommand command)
            => Architecture.Instance.TrySendCommand(command, out _);

        protected bool TrySendCommand<TResult>(ICommand<TResult> command, out TResult? result,
            out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out result, out canExecuteResult);

        protected bool TrySendCommand<TResult>(ICommand<TResult> command, out TResult? result)
            => Architecture.Instance.TrySendCommand(command, out result, out _);

        protected IAggregateRootHandle<T> GetAggregateRootHandle<T>() where T : class, IAggregateRoot
            => Architecture.Instance.GetAggregateRootHandle<T>();
    }
}
