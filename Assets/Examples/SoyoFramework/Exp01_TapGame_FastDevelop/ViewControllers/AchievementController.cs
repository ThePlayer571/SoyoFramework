using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using UnityEngine;

namespace Examples.SoyoFramework.Exp01_TapGame_FastDevelop.ViewControllers
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