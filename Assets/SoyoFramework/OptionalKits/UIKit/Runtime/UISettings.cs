using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    public class UISettings : ScriptableObject
    {
        public List<UIPanelConfig> PanelConfigs;
        public RenderMode CanvasRenderMode = RenderMode.ScreenSpaceOverlay;
    }

    [Serializable]
    public class UIPanelConfig
    {
        public string PanelName;
        public AssetReference PrefabReference;
        public UIPanelLayer Layer;
    }

    public enum UIPanelLayer
    {
        Common = 0,
        PopUI = 1,
        Transition = 2
    }
}