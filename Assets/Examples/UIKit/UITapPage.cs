using System;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;

namespace Examples.UIKit
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

        protected override bool HandleUICommand(UICommand command)
        {
            switch (command)
            {
                case UITapCommand:
                    GetContext<UIScoreContext>().Score.Value++;
                    return true;
                case UIPauseCommand:
                    var context = GetContext<UIScoreContext>();
                    context.IsPaused.Value = !context.IsPaused.Value;
                    return true;
            }

            return false;
        }

        public override IArchitecture RelyingArchitecture => null;
    }

    [Serializable]
    public class UIScoreContext : IUIContext
    {
        public BindableProperty<int> Score = new();
        public BindableProperty<bool> IsPaused = new();
    }
}