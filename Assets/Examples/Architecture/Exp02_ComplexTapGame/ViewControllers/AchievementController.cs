using SoyoFramework.Framework.Examples.Exp02_ComplexTapGame.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.Framework.Examples.Exp02_ComplexTapGame.ViewControllers
{
    public class AchievementController : AbstractVController
    {
        protected override void OnInit()
        {
            // 获得成就通知
            this.RegisterEvent<OnGainAchievement>(e => { $"获得成就：{e.AchievementName}".LogInfo(); });
        }
    }
}