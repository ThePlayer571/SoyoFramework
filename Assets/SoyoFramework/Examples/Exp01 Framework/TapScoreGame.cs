using SoyoFramework.Framework.Runtime.Core;
using UnityEngine;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public class TapScoreGame : AbstractArchitecture
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitArchitecture()
        {
            var architecture = new TapScoreGame();
            architecture.Init();
        }

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