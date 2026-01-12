using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Pages
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
}