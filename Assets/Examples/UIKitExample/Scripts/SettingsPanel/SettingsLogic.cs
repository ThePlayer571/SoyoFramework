using Examples.UIKitExample.Scripts.MainGame;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;

namespace Examples.UIKitExample.Scripts.SettingsPanel
{
    public class SettingsLogic : UIPageLogic
    {
        public SettingsLogic(IUIViewHost host, IMainModel mainModel) : base(host)
        {
            _MainModel = mainModel;
        }

        private IMainModel _MainModel;
        private SettingsContext _SettingsContext;

        public override void OnInit()
        {
            _SettingsContext = Host.GetContext<ISettingsContext>() as SettingsContext;

            _MainModel.UserName.RegisterWithInitValue(userName => { _SettingsContext.userName = userName; });
        }

        public override bool HandleUICommand(UICommand command)
        {
            switch (command)
            {
                case ToggleOpenSettingsPanelCommand:
                    _SettingsContext.panelOpened.Value = !_SettingsContext.panelOpened.Value;
                    return true;
            }

            return false;
        }

        public override bool HandleUICommand<TResult>(UICommand<TResult> command, out TResult result)
        {
            result = default;
            return false;
        }


        public override void OnClose()
        {
        }
    }
}