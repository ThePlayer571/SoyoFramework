using SoyoFramework.Examples.Exp01_Framework.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.LogKit;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.Examples.Exp01_Framework.ViewControllers
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