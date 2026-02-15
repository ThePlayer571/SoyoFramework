using Examples.UIKitExample.Scripts.MainGame;
using Examples.UIKitExample.Scripts.TapGame;
using SoyoFramework.Framework.Runtime.Core;

namespace Examples.UIKitExample.Scripts
{
    public class TwoGames : Architecture<TwoGames>
    {
        protected override void OnInit()
        {
            RegisterModel<ITapGameModel>(new TapGameModel());
            RegisterModel<IMainModel>(new MainModel());
        }

        protected override void OnDeinit()
        {
        }
    }
}