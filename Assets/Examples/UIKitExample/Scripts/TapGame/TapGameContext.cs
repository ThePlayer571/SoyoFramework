using System;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;

namespace Examples.UIKitExample.Scripts.TapGame
{
    [Serializable]
    public class TapGameContext : IUIContext
    {
        public BindableProperty<int> Score = new();
    }
}