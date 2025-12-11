using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.LogKit;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public interface IAchievementSystem : ISystem
    {
    }

    public class AchievementSystem : AbstractSystem, IAchievementSystem
    {
        private bool _achievement_TapNoob_Unlocked = false;
        private bool _achievement_TapMaster_Unlocked = false;
        private bool _achievement_TapLegend_Unlocked = false;


        protected override void OnInit()
        {
            this.RegisterEvent<OnScoreChanged>(newScore =>
            {
                if (!_achievement_TapNoob_Unlocked && newScore.NewScore <= -10)
                {
                    _achievement_TapNoob_Unlocked = true;
                    "解锁成就：点击菜鸟".LogInfo();
                }

                if (!_achievement_TapMaster_Unlocked && newScore.NewScore >= 10)
                {
                    _achievement_TapMaster_Unlocked = true;
                    "解锁成就：点击大师".LogInfo();
                }

                if (!_achievement_TapLegend_Unlocked && newScore.NewScore >= 50)
                {
                    _achievement_TapLegend_Unlocked = true;
                    "解锁成就：点击传奇".LogInfo();
                }
            });
        }
    }
}