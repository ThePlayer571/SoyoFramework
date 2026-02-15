using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.OptionalKits.UIKit.Runtime;

namespace Examples.UIKitExample.Scripts.MainGame
{
    public class SwitchToMainPageCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            UIKit.ClosePage("TapGamePage");
            UIKit.ClosePage("BubbleGamePage");

            UIKit.OpenPageAsync("MainPage").Forget();
            this.SendEvent<OnSwitchToMainPage>();
        }

        public override CanExecuteResult CanExecute()
        {
            var mainPage = UIKit.GetPage("MainPage");

            if (mainPage != null)
            {
                return CanExecuteResult.FailReason("MainPage已打开");
            }

            return CanExecuteResult.Success;
        }
    }
}