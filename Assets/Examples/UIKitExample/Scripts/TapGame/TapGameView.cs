using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using SoyoFramework.ToolKits.Runtime.FluentAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.UIKitExample.Scripts.TapGame
{
    public class TapGameView : UIView
    {
        [SerializeField] private Text ScoreText;
        [SerializeField] private Button AddBtn;

        [SerializeField] private Button WildModeBtn;
        [SerializeField] private Text WildModeHintText;

        private TapGameContext _TapGameContext;

        protected override void OnInit()
        {
            _TapGameContext = Host.GetContext<TapGameContext>();

            // View
            _TapGameContext.Score.RegisterWithInitValue(val =>
            {
                //
                ScoreText.text = val.ToString();
            }).UnRegisterWhenGameObjectDestroyed(this);

            // 输入
            AddBtn.onClick.Register(() =>
            {
                //
                Host.SubmitCommand(new AddScoreCommand());
            }).UnRegisterWhenGameObjectDestroyed(this);

            WildModeBtn.onClick.Register(() =>
            {
                UniTask.Create(async () =>
                {
                    Host.SubmitCommand<UniTask<bool>>(new EnterWildModeCommand(), out var task);
                    // 禁止继续与狂野模式交互
                    WildModeBtn.interactable = false;
                    // better: 与Context强绑定，而不是在这里写逻辑

                    WildModeHintText.text = "正在进入狂野模式...";
                    var isSuccess = await task;
                    if (isSuccess)
                    {
                        WildModeHintText.text = "进入狂野模式成功！";
                    }
                    else
                    {
                        WildModeHintText.text = "进入狂野模式失败！请重试";
                        WildModeBtn.interactable = true;
                    }
                });
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        protected override void OnClose()
        {
        }
    }
}