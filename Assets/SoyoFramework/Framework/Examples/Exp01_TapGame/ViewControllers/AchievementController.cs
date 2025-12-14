using SoyoFramework.Framework.Examples.Exp01_TapGame.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.Framework.Examples.Exp01_TapGame.ViewControllers
{
    public class AchievementController : ViewController
    {
        protected override void OnInit()
        {
            // 获得成就通知
            this.RegisterEvent<OnGainAchievement>(e => { $"获得成就：{e.AchievementName}".LogInfo(); });
        }
    }
}