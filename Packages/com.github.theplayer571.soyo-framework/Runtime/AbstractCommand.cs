namespace SoyoFramework
{
    public abstract class AbstractCommand : ICommand
    {
        protected void SendCommand(ICommand command) => Architecture.Instance.SendCommand(command);

        protected TResult SendCommand<TResult>(ICommand<TResult> command)
            => Architecture.Instance.SendCommand(command);

        protected bool TrySendCommand(ICommand command, out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out canExecuteResult);

        protected bool TrySendCommand(ICommand command)
            => Architecture.Instance.TrySendCommand(command, out _);

        protected bool TrySendCommand<T>(ICommand<T> command, out T? result,
            out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out result, out canExecuteResult);

        protected bool TrySendCommand<T>(ICommand<T> command, out T? result)
            => Architecture.Instance.TrySendCommand(command, out result, out _);

        protected IAggregateRootHandle<T> GetAggregateRootHandle<T>() where T : class, IAggregateRoot
            => Architecture.Instance.GetAggregateRootHandle<T>()!;

        protected bool RegisterAggregateRoot<T>(T aggregateRoot) where T : class, IAggregateRoot
            => Architecture.Instance.RegisterAggregateRoot(aggregateRoot);

        protected bool UnregisterAggregateRoot<T>() where T : class, IAggregateRoot
            => Architecture.Instance.UnregisterAggregateRoot<T>();

        protected abstract void OnExecute();

        void ICommand.Execute()
        {
            OnExecute();
        }

        public virtual CanExecuteResult CanExecute()
        {
            return CanExecuteResult.Success;
        }
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        protected void SendCommand(ICommand command) => Architecture.Instance.SendCommand(command);

        protected T SendCommand<T>(ICommand<T> command)
            => Architecture.Instance.SendCommand(command);

        protected bool TrySendCommand(ICommand command, out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out canExecuteResult);

        protected bool TrySendCommand(ICommand command)
            => Architecture.Instance.TrySendCommand(command, out _);

        protected bool TrySendCommand<T>(ICommand<T> command, out T? result,
            out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out result, out canExecuteResult);

        protected bool TrySendCommand<T>(ICommand<T> command, out T? result)
            => Architecture.Instance.TrySendCommand(command, out result, out _);

        protected IAggregateRootHandle<T> GetAggregateRootHandle<T>() where T : class, IAggregateRoot
            => Architecture.Instance.GetAggregateRootHandle<T>()!;

        protected bool RegisterAggregateRoot<T>(T aggregateRoot) where T : class, IAggregateRoot
            => Architecture.Instance.RegisterAggregateRoot(aggregateRoot);

        protected bool UnregisterAggregateRoot<T>() where T : class, IAggregateRoot
            => Architecture.Instance.UnregisterAggregateRoot<T>();

        protected abstract TResult OnExecute();

        TResult ICommand<TResult>.Execute()
        {
            return OnExecute();
        }

        void ICommand.Execute()
        {
            ((ICommand<TResult>)this).Execute();
        }

        public virtual CanExecuteResult CanExecute()
        {
            return CanExecuteResult.Success;
        }
    }
}
