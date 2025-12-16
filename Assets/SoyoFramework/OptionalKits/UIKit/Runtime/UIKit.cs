using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    public static class UIKit
    {
        private static bool _isInitialized = false;

        public static async UniTask InitAsync()
        {
            if (_isInitialized)
            {
                "UIKit已经初始化".LogError();
                return;
            }

            // try
            // {
            // todo UISettings配置引导
            // 读取UISettings
            var uiSettingsHandle = Addressables.LoadAssetAsync<UISettings>("UISettings");
            await uiSettingsHandle.ToUniTask();
            var uiSettings = uiSettingsHandle.Result;

            // 实例化UIRoot
            var uiRootPrefab = Resources.Load<GameObject>("UIRoot");
            var uiRoot = uiRootPrefab.Instantiate().GetComponent<UIRoot>();
            uiRoot.UIManager.Init(uiSettings);
            uiRoot.DontDestroyOnLoad();

            _isInitialized = true;
            // }
            // catch (Exception ex)
            // {
            //     Debug.LogError($"UIKit初始化失败: {ex.Message}");
            //     throw;
            // }
        }

        public static void PutUICameraIntoStack(Camera camera)
        {
            var data = camera.GetUniversalAdditionalCameraData();
            data.cameraStack.Add(UIRoot.Instance.UICamera);
        }

        public static UIRoot GetUIRoot() => UIRoot.Instance;

        public static async UniTask<T> OpenPanelAsync<T>(string panelName, object initData = null) where T : class, IUIMainPanel
        {
            return await UIManager.Instance.OpenPanelAsync<T>(panelName, initData);
        }

        public static async UniTask<IUIMainPanel> OpenPanelAsync(string panelName, object initData = null)
        {
            return await UIManager.Instance.OpenPanelAsync<IUIMainPanel>(panelName, initData);
        }

        public static void ClosePanel(string panelName)
        {
            UIManager.Instance.ClosePanel(panelName);
        }

        public static T GetPanel<T>(string panelName) where T : class, IUIMainPanel
        {
            return UIManager.Instance.GetPanel<T>(panelName);
        }

        public static class Stack
        {
            public static void Push(IStackablePanel panel)
            {
                UIManager.Instance.StackPush(panel);
            }

            public static void Remove(IStackablePanel panel)
            {
                UIManager.Instance.StackRemove(panel);
            }

            public static IStackablePanel Pop()
            {
                return UIManager.Instance.StackPop();
            }

            public static IStackablePanel Peek()
            {
                return UIManager.Instance.StackPeek();
            }

            public static void Clear()
            {
                UIManager.Instance.StackClear();
            }

            public static int Count => UIManager.Instance.StackCount;
        }
    }
}