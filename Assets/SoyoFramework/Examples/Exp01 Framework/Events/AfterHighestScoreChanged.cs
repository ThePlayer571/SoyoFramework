using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.Examples.Exp01_Framework.Events
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