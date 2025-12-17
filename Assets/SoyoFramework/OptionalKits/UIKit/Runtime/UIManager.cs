using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
using UnityEngine;
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

            // RenderMode
            uiRoot.Canvas.renderMode = uiSettings.CanvasRenderMode;
            if (uiSettings.CanvasRenderMode == RenderMode.ScreenSpaceCamera)
            {
                uiRoot.UICamera.SetActive(true);
            }

            // Page信息存储
            foreach (var config in uiSettings.PageConfigs)
            {
                _pageConfigs.Add(config.PageName, config);
            }
        }

        private Dictionary<string, UIPageConfig> _pageConfigs = new();
        private Dictionary<string, ActiveUIPageMetaData> _activeUIPageMetaData = new();

        #region Page api

        public async UniTask<T> OpenPageAsync<T>(string pageName) where T : UIPage
        {
            if (_activeUIPageMetaData.ContainsKey(pageName))
            {
                $"UIPage: {pageName} 已经打开".LogError();
                return null;
            }

            if (!_pageConfigs.TryGetValue(pageName, out var pageConfig))
            {
                $"UIPage: 找不到 {pageName} 的配置信息".LogError();
                return null;
            }

            var handle = pageConfig.PrefabReference.InstantiateAsync(UIRoot.Canvas.transform);
            await handle.ToUniTask();

            var pageInstance = handle.Result.GetComponent<UIPage>();
            if (pageInstance == null)
            {
                $"UIPage: {pageName} 的预制体上没有挂载UIPage组件".LogError();
                handle.Release();
                return null;
            }

            // 调整层级，保证PanelOrder高的在上层
            SetPageSiblingByPanelOrder(pageInstance.transform, pageConfig.PanelOrder, UIRoot.Canvas.transform,
                _activeUIPageMetaData);

            // 通过所有检查，开始初始化
            pageInstance.Init();

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
            // 移除记录
            if (!_activeUIPageMetaData.Remove(pageName, out var metaData))
            {
                $"UIPage: {pageName} 未打开".LogWarning();
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
                $"UIPage: {pageName} 未打开".LogWarning();
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

        /// <summary>
        /// 调整UIPage的层级，使PanelOrder高的页面在上层
        /// </summary>
        /// <param name="pageTransform">新开UIPage的Transform</param>
        /// <param name="panelOrder">新UIPage的PanelOrder</param>
        /// <param name="parent">Canvas.transform</param>
        /// <param name="activeMetaData">当前激活页面信息</param>
        private static void SetPageSiblingByPanelOrder(Transform pageTransform, int panelOrder, Transform parent,
            Dictionary<string, ActiveUIPageMetaData> activeMetaData)
        {
            int highestLowerOrderSiblingIndex = -1;
            int maxPanelOrderBelowCurrent = int.MinValue;

            // 查找插入位置: 找到所有比当前PanelOrder小的子节点中最大siblingIndex
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                UIPage childPage = child.GetComponent<UIPage>();
                if (childPage != null)
                {
                    foreach (var pair in activeMetaData)
                    {
                        if (pair.Value.PageInstance == childPage)
                        {
                            int childPanelOrder = pair.Value.PanelConfig.PanelOrder;
                            if (childPanelOrder <= panelOrder && childPanelOrder > maxPanelOrderBelowCurrent)
                            {
                                maxPanelOrderBelowCurrent = childPanelOrder;
                                highestLowerOrderSiblingIndex = i;
                            }

                            break;
                        }
                    }
                }
            }

            if (highestLowerOrderSiblingIndex == -1)
            {
                // 没有比它PanelOrder小的，放到最前面（最底层）
                pageTransform.SetSiblingIndex(0);
            }
            else
            {
                // 插在所有比它小的层的后面
                pageTransform.SetSiblingIndex(highestLowerOrderSiblingIndex + 1);
            }
        }
    }
}