using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "SoyoFramework/UIKit/UISettings", order = 1)]
    public class UISettings : ScriptableObject
    {
        public List<UIPageConfig> PageConfigs;
        public RenderMode CanvasRenderMode = RenderMode.ScreenSpaceOverlay;
    }

    [Serializable]
    public class UIPageConfig
    {
        public string PageName;
        public AssetReference PrefabReference;
        public int PanelOrder;
    }
}