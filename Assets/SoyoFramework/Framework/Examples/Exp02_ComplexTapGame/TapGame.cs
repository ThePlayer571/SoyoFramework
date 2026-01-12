using SoyoFramework.Framework.Examples.Exp02_ComplexTapGame.ViewControllers;
using SoyoFramework.Framework.Runtime.Core;

namespace SoyoFramework.Framework.Examples.Exp02_ComplexTapGame
{
    public class TapGame : AbstractArchitecture
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