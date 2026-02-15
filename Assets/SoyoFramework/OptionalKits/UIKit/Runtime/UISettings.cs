using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "SoyoFramework/UIKit/UISettings", order = 1)]
    public class UISettings : ScriptableObject
    {
        [Header("Page Configurations")] public List<UIPageConfig> PageConfigs;

        [Header("Canvas Settings")] public GameObject UIRoot;
        [Tooltip("UI排序层，在下的代表在上层")] public List<string> LayerKeys;

#if UNITY_EDITOR
        /// <summary>
        /// 创建或重置时自动设置默认值
        /// </summary>
        private void Reset()
        {
            UIRoot = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(Global.DefaultUIRootPath);
        }
#endif
    }

    [Serializable]
    public class UIPageConfig
    {
        public string PageName;
        public AssetReferenceGameObject PrefabReference;
        public string LayerKey;
    }
}