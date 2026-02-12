using SoyoFramework.Framework.Runtime.Utils;

namespace Examples.SoyoFramework.Exp02_TapGame.Events
{
    public class OnGainAchievement : IEvent
    {
        public string AchievementName { get; }

        public OnGainAchievement(string achievementName)
        {
            AchievementName = achievementName;
        }
    }
}