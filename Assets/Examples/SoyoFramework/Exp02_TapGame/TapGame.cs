using Examples.SoyoFramework.Exp02_TapGame.Models;
using Examples.SoyoFramework.Exp02_TapGame.Systems;
using Examples.SoyoFramework.Exp02_TapGame.ViewControllers;
using Examples.SoyoFramework.Exp02_TapGame.ViewModels;
using SoyoFramework.Framework.Runtime.Core;

namespace Examples.SoyoFramework.Exp02_TapGame
{
    public class TapGame : Architecture<TapGame>
    {
        protected override void OnInit()
        {
            this.RegisterModel<IScoreModel>(new ScoreModel());
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
            this.RegisterVController(new AchievementController());
            this.RegisterTool<IScoreViewModel>(new ScoreViewModel());
        }

        protected override void OnDeinit()
        {
        }
    }
}