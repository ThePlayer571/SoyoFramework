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
            $"Hello, World!: {intContext.Value.Value}".LogInfo();
            Host.SubmitCommand(new HelloWorldCommand());

            var item1 = new StackItem()
                .WithOnPushed(() => { $"HelloWorldView: Item 1 Pushed".LogInfo(); })
                .WithOnPopped(() => { $"HelloWorldView: Item 1 Popped".LogInfo(); });

            var item2 = new StackItem()
                .WithOnPushed(() => { $"HelloWorldView: Item 2 Pushed".LogInfo(); })
                .WithOnPopped(() => { $"HelloWorldView: Item 2 Popped".LogInfo(); });

            var stackModule = Host.GetModule<StackModule>();
            stackModule.TryPush(item1);
            stackModule.TryPush(item2);
            stackModule.TryPush(item1);

            stackModule.Pop();
            stackModule.Pop();
            stackModule.Pop();
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