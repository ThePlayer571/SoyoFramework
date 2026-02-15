using System;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;

namespace Examples.UIKitExample.Scripts.SettingsPanel
{
    public interface ISettingsContext : IUIContext
    {
        IReadOnlyBindableProperty<bool> PanelOpened { get; }
        string UserName { get; }
    }

    [Serializable]
    public class SettingsContext : ISettingsContext
    {
        public BindableProperty<bool> panelOpened = new();
        public string userName;

        // 接口实现
        public IReadOnlyBindableProperty<bool> PanelOpened => panelOpened;
        public string UserName => userName;
    }
}