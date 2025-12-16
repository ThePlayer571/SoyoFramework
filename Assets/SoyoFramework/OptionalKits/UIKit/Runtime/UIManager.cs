using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    internal class UIManager : MonoBehaviour
    {
        #region 生命周期

        public static UIManager Instance { get; private set; }

        public void Init(UISettings uiSettings)
        {
            Instance = this;
            UIRoot.Instance.UICanvas.renderMode = uiSettings.CanvasRenderMode;
            if (uiSettings.CanvasRenderMode == RenderMode.ScreenSpaceCamera)
            {
                UIRoot.Instance.UICamera.SetActive(true);
            }

            foreach (var config in uiSettings.PanelConfigs)
            {
                _panelConfigs.Add(config.PanelName, config);
            }

            StackInit();
        }

        #endregion

        // 变量
        private Dictionary<string, UIPanelConfig> _panelConfigs = new();
        private Dictionary<string, ActiveUIPanelMetaData> _activeUIPanelMetaData = new();

        private EasyEvent<IUIPanel> OnCloseUIPanelBeforeEvent = new();


        private class ActiveUIPanelMetaData
        {
            public IUIMainPanel Instance;
            public UIPanelConfig PanelConfig;
            public AsyncOperationHandle<GameObject> Handle;
        }

        #region 常规api

        public async UniTask<T> OpenPanelAsync<T>(string panelName, object initData) where T : class, IUIMainPanel
        {
            if (_activeUIPanelMetaData.ContainsKey(panelName))
            {
                $"UIPanel:{panelName}已经打开，无法再次打开".LogError();
                return null;
            }

            if (!_panelConfigs.ContainsKey(panelName))
            {
                $"找不对对应的配置信息: {panelName}".LogError();
                return null;
            }

            // 加载数据
            var config = _panelConfigs[panelName];
            var handle = config.PrefabReference.LoadAssetAsync<GameObject>();
            var metaData = new ActiveUIPanelMetaData()
            {
                PanelConfig = config,
                Handle = handle
            };
            _activeUIPanelMetaData.Add(panelName, metaData);

            await handle.ToUniTask();

            // 实例化
            var instanceGO = handle.Result.Instantiate();
            var instance = instanceGO.GetComponent<IUIMainPanel>();

            var layer = config.Layer switch
            {
                UIPanelLayer.Common => UIRoot.Instance.Common,
                UIPanelLayer.PopUI => UIRoot.Instance.PopUI,
                UIPanelLayer.Transition => UIRoot.Instance.Transition,
                _ => throw new ArgumentOutOfRangeException()
            };
            instanceGO.transform.SetParent(layer, false);
            instance.Init(initData);

            metaData.Instance = instance;
            return instance as T;
        }

        public void ClosePanel(string panelName)
        {
            if (!_activeUIPanelMetaData.TryGetValue(panelName, out var metaData))
            {
                $"指定的UIPanel未打开: {panelName}".LogWarning();
                return;
            }

            OnCloseUIPanelBeforeEvent.Trigger(metaData.Instance);

            metaData.Instance.Close();
            metaData.Handle.Release();
            _activeUIPanelMetaData.Remove(panelName);
        }

        public T GetPanel<T>(string panelName) where T : class, IUIMainPanel
        {
            if (!_activeUIPanelMetaData.TryGetValue(panelName, out var metaData))
            {
                $"指定的UIPanel未打开: {panelName}".LogWarning();
                return null;
            }

            var panel = metaData.Instance;
            return panel as T;
        }

        #endregion

        #region 堆栈

        private List<IStackablePanel> _panelStack = new();

        private void StackInit()
        {
            OnCloseUIPanelBeforeEvent.Register(panel =>
            {
                if (panel is IStackablePanel stackPanel && _panelStack.Contains(stackPanel))
                {
                    StackRemove(stackPanel);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public void StackPush(IStackablePanel panel)
        {
            if (_panelStack.Contains(panel))
            {
                $"UIStack中已经存在该UIPanel".LogError();
                return;
            }

            _panelStack.Add(panel);
            panel.OnPushed();
        }

        public void StackRemove(IStackablePanel panel)
        {
            if (!_panelStack.Contains(panel))
            {
                // $"UIStack中不存在该UIPanel".LogError();
                return;
            }

            _panelStack.Remove(panel);
            if ((panel as UnityEngine.Object) != null) panel.OnPopped();
        }

        public IStackablePanel StackPop()
        {
            if (_panelStack.Count == 0)
            {
                $"UIStack为空".LogError();
                return null;
            }

            var popped = _panelStack.Pop();
            if ((popped as UnityEngine.Object) != null) popped.OnPopped();
            return popped;
        }

        public IStackablePanel StackPeek()
        {
            if (_panelStack.Count == 0)
            {
                $"UIStack为空".LogError();
                return null;
            }

            return _panelStack.Last();
        }

        public void StackClear()
        {
            while (_panelStack.Count > 0)
            {
                var popped = _panelStack.Pop();
                if ((popped as UnityEngine.Object) != null) popped.OnPopped();
            }
        }

        public int StackCount => _panelStack.Count;

        #endregion
    }
}