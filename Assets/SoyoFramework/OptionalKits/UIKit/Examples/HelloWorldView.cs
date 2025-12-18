using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Modules;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class HelloWorldView : UIView
    {
        protected override void OnInit()
        {
            var intContext = Host.GetContext<IntContext>();
            intContext.Value.Register(v => $"new Val: {v}".LogInfo());
        }

        protected override void OnClose()
        {
        }
    }

    public class HelloWorldCommand : ICommand
    {
        public IArchitecture AttachedArchitecture { get; set; }

        public void Execute()
        {
        }
    }
}