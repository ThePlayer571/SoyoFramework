using System;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class UITapPage : UIPage
    {
        protected override void Configure()
        {
            RegisterContext<UIScoreContext>(new UIScoreContext());
        }

        protected override void OnInit()
        {
            // 补充输入
        }

        protected override void OnClose()
        {
        }

        public override void SubmitCommand(ICommand command)
        {
            switch (command)
            {
                case UITapCommand:
                    GetContext<UIScoreContext>().Score.Value++;
                    break;
                case UIPauseCommand:
                    var context = GetContext<UIScoreContext>();
                    context.IsPaused.Value = !context.IsPaused.Value;
                    break;
            }
        }

        public override TResult SubmitCommand<TResult>(ICommand<TResult> command)
        {
            return default;
        }
    }

    [Serializable]
    public class UIScoreContext : IUIContext
    {
        public BindableProperty<int> Score = new();
        public BindableProperty<bool> IsPaused = new();
    }
}