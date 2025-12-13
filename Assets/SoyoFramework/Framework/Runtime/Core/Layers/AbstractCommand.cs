namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class AbstractCommand : ICommand
    {
        protected abstract void OnExecute();

        void ICommand.Execute()
        {
            OnExecute();
        }

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        protected abstract TResult OnExecute();

        TResult ICommand<TResult>.Execute()
        {
            return OnExecute();
        }

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
    }
}