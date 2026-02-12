using Examples.SoyoFramework.Exp02_TapGame.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;

namespace Examples.SoyoFramework.Exp02_TapGame.ViewControllers
{
    public class AchievementController : AbstractVController
    {
        protected override void OnInit()
        {
            // 获得成就通知
            this.RegisterEvent<OnGainAchievement>(e => { Debug.Log($"获得成就：{e.AchievementName}"); });
        }
    }
}