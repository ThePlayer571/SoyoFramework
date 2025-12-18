using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class UITapView : UIView
    {
        [SerializeField] private Text ScoreText;
        [SerializeField] private Button AddBtn;

        protected override void OnInit()
        {
            Host.GetContext<UIScoreContext>().Score.RegisterWithInitValue(val =>
            {
                //
                ScoreText.text = val.ToString();
            }).UnRegisterWhenGameObjectDestroyed(this);

            AddBtn.onClick.AddListener(() => { Host.SubmitCommand(new UITapCommand()); });
        }

        protected override void OnClose()
        {
            AddBtn.onClick.RemoveAllListeners();
        }
    }
}