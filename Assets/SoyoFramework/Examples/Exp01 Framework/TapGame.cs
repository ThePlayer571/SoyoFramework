using SoyoFramework.Examples.Exp01_Framework.ViewControllers;
using SoyoFramework.Framework.Runtime.Core;
using UnityEngine;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public class TapGame : AbstractArchitecture
    {
        protected override void OnInit()
        {
            this.RegisterModel<IScoreModel>(new ScoreModel());
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
            this.RegisterViewController(new AchievementController());
        }

        protected override void OnDeinit()
        {
        }
    }
}