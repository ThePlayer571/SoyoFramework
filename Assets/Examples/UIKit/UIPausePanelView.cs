using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using SoyoFramework.ToolKits.Runtime.FluentAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.UIKit
{
    public class UIPausePanelView : UIView
    {
        [SerializeField] private Button PauseBtn;
        [SerializeField] private Transform PausePanel;

        protected override void OnInit()
        {
            PauseBtn.onClick.AddListener(() => Host.SubmitCommand(new UIPauseCommand()));

            Host.GetContext<UIScoreContext>().IsPaused.RegisterWithInitValue(isPaused =>
            {
                PausePanel.gameObject.SetActive(isPaused);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        protected override void OnClose()
        {
            PauseBtn.onClick.RemoveAllListeners();
        }
    }
}