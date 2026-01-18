using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.Framework.Examples.Exp01_TapGame.Events
{
    public class AfterScoreChanged : IEvent
    {
        public int Score;
    }

    // 获得成就事件
    public class OnGainAchievement : IEvent
    {
        public string AchievementName;
    }
}