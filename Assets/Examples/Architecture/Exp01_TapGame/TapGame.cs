using SoyoFramework.Framework.Examples.Exp01_TapGame.ViewControllers;
using SoyoFramework.Framework.Runtime.Core;

namespace SoyoFramework.Framework.Examples.Exp01_TapGame
{
    public class TapGame : Architecture<TapGame>
    {
        protected override void OnInit()
        {
            this.RegisterModel<IScoreModel>(new ScoreModel());
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
            this.RegisterVController(new AchievementController());
        }

        protected override void OnDeinit()
        {
        }
    }
}