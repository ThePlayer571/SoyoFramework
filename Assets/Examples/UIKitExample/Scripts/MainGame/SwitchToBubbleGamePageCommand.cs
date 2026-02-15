using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.OptionalKits.UIKit.Runtime;

namespace Examples.UIKitExample.Scripts.MainGame
{
    public class SwitchToBubbleGamePageCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            UIKit.ClosePage("MainPage");

            UIKit.OpenPageAsync("BubbleGamePage").Forget();
            this.SendEvent<OnSwitchToBubbleGamePage>();
        }

        public override CanExecuteResult CanExecute()
        {
            var mainPage = UIKit.GetPage("MainPage");

            if (mainPage == null)
            {
                return CanExecuteResult.FailReason("不在MainPage，无法打开BubbleGamePage");
            }

            return CanExecuteResult.Success;
        }
    }
}