using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.OptionalKits.UIKit.Runtime;

namespace Examples.UIKitExample.Scripts.MainGame
{
    public class SwitchToTapGamePageCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            UIKit.ClosePage("MainPage");

            UIKit.OpenPageAsync("TapGamePage").Forget();
            this.SendEvent<OnSwitchToTapGamePage>();
        }

        public override CanExecuteResult CanExecute()
        {
            var mainPage = UIKit.GetPage("MainPage");

            if (mainPage == null)
            {
                return CanExecuteResult.FailReason("不在MainPage，无法打开TapGamePage");
            }

            return CanExecuteResult.Success;
        }
    }
}