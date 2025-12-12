using SoyoFramework.Framework.Runtime.Core;
using UnityEngine;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public class TapGame : AbstractArchitecture
    {
        protected override void OnInit()
        {
            this.RegisterModel<IScoreModel>(new ScoreModel());
            this.RegisterService<IScoreService>(new ScoreService());
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
        }

        protected override void OnDeinit()
        {
        }
    }
}