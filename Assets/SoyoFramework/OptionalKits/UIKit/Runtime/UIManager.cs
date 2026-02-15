using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using SoyoFramework.ToolKits.Runtime.FluentAPI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    internal class UIManager : MonoBehaviour
    {
        // 初始化
        public void Init(UIRoot uiRoot, UISettings uiSettings)
        {
            UIRoot = uiRoot;
            Instance = this;

            // UICamera自动激活
            if (UIRoot.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                uiRoot.UICamera.SetActive(true);
            }

            // 创建层级容器
            UIRoot.InitLayers(uiSettings.LayerKeys);

            // Page信息存储
            foreach (var config in uiSettings.PageConfigs)
            {
                _pageConfigs.Add(config.PageName, config);
            }
        }

        private Dictionary<string, UIPageConfig> _pageConfigs = new();
        private Dictionary<string, ActiveUIPageMetaData> _activeUIPageMetaData = new();
        private ILog _logger = new PrefixLogger("[UIManager]");


        #region Page api

        public async UniTask<T> OpenPageAsync<T>(string pageName, PageOpenSettings openSettings) where T : UIPage
        {
            if (_activeUIPageMetaData.ContainsKey(pageName))
            {
                $"UIPage: {pageName} 已经打开".LogError(_logger);
                return null;
            }

            if (!_pageConfigs.TryGetValue(pageName, out var pageConfig))
            {
                $"UIPage: 找不到 {pageName} 的配置信息".LogError(_logger);
                return null;
            }

            // 检查LayerKey是否有效
            var layerTransform = UIRoot.GetLayerTransform(pageConfig.LayerKey);
            if (layerTransform == null)
            {
                $"UIPage: {pageName} 的LayerKey '{pageConfig.LayerKey}' 未注册".LogError(_logger);
                return null;
            }

            if (!pageConfig.PrefabReference.RuntimeKeyIsValid())
            {
                $"UIPage: {pageName} 的PrefabReference未设置或Key无效".LogError(_logger);
                return null;
            }

            var handle = Addressables.InstantiateAsync(pageConfig.PrefabReference, layerTransform);
            await handle.ToUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            {
                $"UIPage: {pageName} 的预制体实例化失败".LogError(_logger);
                return null;
            }

            var pageInstance = handle.Result.GetComponent<UIPage>();
            if (pageInstance == null)
            {
                $"UIPage: {pageName} 的预制体上没有挂载UIPage组件".LogError(_logger);
                handle.Release();
                return null;
            }

            // 通过所有检查，开始初始化
            pageInstance.Init(openSettings);

            // 记录数据
            var metaData = new ActiveUIPageMetaData
            {
                PageInstance = pageInstance,
                PanelConfig = pageConfig,
                Handle = handle
            };
            _activeUIPageMetaData.Add(pageName, metaData);

            return pageInstance as T;
        }

        public void ClosePage(string pageName)
        {
            // 检查是否打开
            if (!_activeUIPageMetaData.Remove(pageName, out var metaData))
            {
                return;
            }

            metaData.PageInstance.Close();

            // 释放资源
            metaData.Handle.Release();
        }

        public T GetPage<T>(string pageName) where T : UIPage
        {
            if (!_activeUIPageMetaData.TryGetValue(pageName, out var metaData))
            {
                return null;
            }

            return metaData.PageInstance as T;
        }

        #endregion

        #region 其他数据读取

        public static UIManager Instance { get; private set; }
        public UIRoot UIRoot { get; private set; }

        #endregion

        /// <summary>
        /// 正在打开的UIPage的数据
        /// </summary>
        private class ActiveUIPageMetaData
        {
            public UIPage PageInstance;
            public UIPageConfig PanelConfig;
            public AsyncOperationHandle<GameObject> Handle;
        }
    }
}