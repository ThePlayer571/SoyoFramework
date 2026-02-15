using Examples.UIKitExample.Scripts.MainGame;
using Examples.UIKitExample.Scripts.SettingsPanel;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.UIKitExample.Scripts.MainPage
{
    // bad: 这个类乱到令人发指了，不在示例范围内，别学
    public class MainPage : UIPage
    {
        [SerializeField] private Button StartTapGameBtn;
        [SerializeField] private Button StartBubbleGameBtn;

        protected override void Configure()
        {
            RegisterContext<ISettingsContext>(new SettingsContext());
        }

        protected override void OnInit()
        {
            StartTapGameBtn.onClick.AddListener(() => { SubmitCommand(new SwitchToTapGamePageCommand()); });

            StartBubbleGameBtn.onClick.AddListener(() => { SubmitCommand(new SwitchToBubbleGamePageCommand()); });

            RegisterLogic(new SettingsLogic(this, this.GetModel<IMainModel>()));
        }

        protected override void OnClose()
        {
            StartTapGameBtn.onClick.RemoveAllListeners();
            StartBubbleGameBtn.onClick.RemoveAllListeners();
        }

        protected override bool HandleUICommand(UICommand command)
        {
            return false;
        }

        protected override bool HandleUICommand<TResult>(UICommand<TResult> command, out TResult result)
        {
            result = default;
            return false;
        }

        public override IArchitecture RelyingArchitecture => TwoGames.Instance;
    }
}