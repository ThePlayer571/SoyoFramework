using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Events;
using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Models;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Systems
{
    public interface IAchievementSystem : ISystem
    {
    }

    public class AchievementSystem : AbstractSystem, IAchievementSystem
    {
        private IScoreModel _ScoreModel;

        protected override void OnInit()
        {
            _ScoreModel = this.GetModel<IScoreModel>();

            _ScoreModel.HighestScore.Register(newHigh =>
            {
                if (newHigh == 10)
                {
                    this.SendEvent(new OnGainAchievement { AchievementName = "点击大师" });
                }
                else if (newHigh == 20)
                {
                    this.SendEvent(new OnGainAchievement { AchievementName = "点击宗师" });
                }
            });
        }
    }
}