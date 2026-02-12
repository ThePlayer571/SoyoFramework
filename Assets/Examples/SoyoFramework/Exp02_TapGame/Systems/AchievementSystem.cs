using Examples.SoyoFramework.Exp02_TapGame.Events;
using Examples.SoyoFramework.Exp02_TapGame.Models;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace Examples.SoyoFramework.Exp02_TapGame.Systems
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
                    this.SendEvent(new OnGainAchievement("点击大师"));
                }
                else if (newHigh == 20)
                {
                    this.SendEvent(new OnGainAchievement("点击宗师"));
                }
            });
        }
    }
}