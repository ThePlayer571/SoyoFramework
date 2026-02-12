using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Models;
using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Systems;
using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.ViewControllers;
using SoyoFramework.Framework.Runtime.Core;

namespace Examples.SoyoFramework.Exp01_TapGame_FastDevelop
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