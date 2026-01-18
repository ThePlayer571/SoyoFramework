using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using SoyoFramework.ToolKits.Runtime.FluentAPI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    public static class UIKit
    {
        #region 初始化方法

        private static bool _isInitialized = false;

        public static void Init(UISettings uiSettings)
        {
            if (_isInitialized)
            {
                "UIKit已经初始化".LogError();
                return;
            }

            _isInitialized = true;

            // 创建UIRoot
            var uiRoot = uiSettings.UIRoot.Instantiate().GetComponent<UIRoot>();
            uiRoot.DontDestroyOnLoad();

            // 初始化UIManager（更多初始化内容都在UIManager内部完成）
            var uiManager = uiRoot.UIManager;
            uiManager.Init(uiRoot, uiSettings);
        }

        #endregion

        #region Page 生命周期

        public static async UniTask<T> OpenPageAsync<T>(string pageName, PageOpenSettings openSettings = null)
            where T : UIPage
        {
            openSettings ??= new PageOpenSettings();
            return await UIManager.Instance.OpenPageAsync<T>(pageName, openSettings);
        }

        public static async UniTask OpenPageAsync(string pageName, PageOpenSettings openSettings = null)
        {
            openSettings ??= new PageOpenSettings();
            await UIManager.Instance.OpenPageAsync<UIPage>(pageName, openSettings);
        }

        public static T GetPage<T>(string pageName) where T : UIPage
        {
            return UIManager.Instance.GetPage<T>(pageName);
        }

        public static UIPage GetPage(string pageName)
        {
            return UIManager.Instance.GetPage<UIPage>(pageName);
        }

        public static void ClosePage(string pageName)
        {
            UIManager.Instance.ClosePage(pageName);
        }

        #endregion

        #region 其他方法

        public static void PutUICameraIntoStack(Camera camera)
        {
            var data = camera.GetUniversalAdditionalCameraData();
            data.cameraStack.Add(UIManager.Instance.UIRoot.UICamera);
        }
        
        public static UIRoot UIRoot => UIManager.Instance.UIRoot;

        #endregion
    }
}