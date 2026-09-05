namespace SoyoFramework
{
    public abstract class AbstractCommand : ICommand
    {
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
