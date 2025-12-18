using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
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
            var uiRootPrefab = Resources.Load<GameObject>("UIRoot");
            var uiRoot = uiRootPrefab.Instantiate().GetComponent<UIRoot>();
            uiRoot.DontDestroyOnLoad();

            // 初始化UIManager（更多初始化内容都在UIManager内部完成）
            var uiManager = uiRoot.UIManager;
            uiManager.Init(uiRoot, uiSettings);
        }

        #endregion

        #region Page 生命周期

        public static async UniTask<T> OpenPageAsync<T>(string pageName) where T : UIPage
        {
            return await UIManager.Instance.OpenPageAsync<T>(pageName);
        }

        public static async UniTask OpenPageAsync(string pageName)
        {
            await UIManager.Instance.OpenPageAsync<UIPage>(pageName);
        }

        public static T GetPage<T>(string pageName) where T : UIPage
        {
            return UIManager.Instance.GetPage<T>(pageName);
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

        #endregion
    }
}