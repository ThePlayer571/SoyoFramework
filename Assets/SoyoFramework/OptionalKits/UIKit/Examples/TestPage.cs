using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Modules;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class TestPage : UIPage
    {
        protected override void Configure()
        {
            var intContext = new IntContext();
            intContext.Value.SetValueWithoutTrigger(5);
            RegisterContext<IntContext>(intContext);
            
            RegisterModule(new StackModule(this));
        }

        protected override void OnInit()
        {
        }

        protected override void OnClose()
        {
        }

        public override void SubmitCommand(ICommand command)
        {
            $"提交了Command: {command.GetType().FullName}".LogInfo();
        }

        public override TResult SubmitCommand<TResult>(ICommand<TResult> command)
        {
            $"提交了Command: {command.GetType().FullName}".LogInfo();
            return default;
        }
    }

    public class IntContext : IUIContext
    {
        public BindableProperty<int> Value = new();
    }
}