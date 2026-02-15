using Examples.UIKitExample.Scripts.MainGame;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers;
using SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.UIKitExample.Scripts.SettingsPanel
{
    public class SettingsPanelView : UIView
    {
        // 引用
        [Header("引用")] [SerializeField] private RectTransform Root;
        [SerializeField] private InputField UserNameInputField;
        [SerializeField] private Button ApplyButton;
        [SerializeField] private Button CloseButton;
        [SerializeField] private Button LeftButton;
        [SerializeField] private Button BlackMaskButton;

        [SerializeField] private Button OpenPanelButton;

        // 可配置项
        [Header("可配置项")] [SerializeField] private LeftBtnFunctionType LeftBtnFunction;

        private enum LeftBtnFunctionType
        {
            ExitGame = 0,
            SwitchToMainPage = 1,
        }

        // 
        private ISettingsContext _SettingsContext;
        private ViewHelper _ViewHelper;

        protected override void OnInit()
        {
            _SettingsContext = Host.GetContext<ISettingsContext>();

            _ViewHelper = new ViewHelper(false)
                .WithOnShow(() =>
                {
                    Root.gameObject.SetActive(true);
                    UserNameInputField.text = _SettingsContext.UserName;
                })
                .WithOnHide(() => { Root.gameObject.SetActive(false); });

            _ViewHelper.ForceResetView();

            _unRegister = _SettingsContext.PanelOpened.RegisterWithInitValue(isOpen =>
            {
                _ViewHelper.SetView(isOpen);
            });

            // 输入
            OpenPanelButton.onClick.AddListener(() => { Host.SubmitCommand(new ToggleOpenSettingsPanelCommand()); });

            ApplyButton.onClick.AddListener(() =>
            {
                Host.SubmitCommand(new SetUserNameCommand(UserNameInputField.text));
            });

            CloseButton.onClick.AddListener(() => { Host.SubmitCommand(new ToggleOpenSettingsPanelCommand()); });
            BlackMaskButton.onClick.AddListener(() => { Host.SubmitCommand(new ToggleOpenSettingsPanelCommand()); });

            LeftButton.onClick.AddListener(() =>
            {
                switch (LeftBtnFunction)
                {
                    case LeftBtnFunctionType.SwitchToMainPage:
                        Host.SubmitCommand(new SwitchToMainPageCommand());
                        break;
                    case LeftBtnFunctionType.ExitGame:
                        UnityEditor.EditorApplication.isPlaying = false;
                        break;
                }
            });
        }

        private IUnRegister _unRegister;

        protected override void OnClose()
        {
            OpenPanelButton.onClick.RemoveAllListeners();
            ApplyButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
            LeftButton.onClick.RemoveAllListeners();
            BlackMaskButton.onClick.RemoveAllListeners();
            _unRegister.UnRegister();
        }
    }
}