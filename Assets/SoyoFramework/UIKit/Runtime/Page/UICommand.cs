using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.UIKit.Runtime.Page
{
    public class UICommand : ICommand
    {
        public IArchitecture AttachedArchitecture { get; set; } = null;

        void ICommand.Execute(bool ignoreCanExecuteCheck)
        {
            "这是UICommand，应该交与UIPage来处理，而不是发送给Architecture".LogError();
        }

        public virtual CanExecuteResult CanExecute() => CanExecuteResult.Success;
    }

    public class UICommand<TResult> : ICommand<TResult>
    {
        public IArchitecture AttachedArchitecture { get; set; } = null;

        TResult ICommand<TResult>.Execute(bool ignoreCanExecuteCheck)
        {
            "这是UICommand，应该交与UIPage来处理，而不是发送给Architecture".LogError();
            return default;
        }

        void ICommand.Execute(bool ignoreCanExecuteCheck)
        {
            "这是UICommand，应该交与UIPage来处理，而不是发送给Architecture".LogError();
        }

        public virtual CanExecuteResult CanExecute() => CanExecuteResult.Success;
    }
}