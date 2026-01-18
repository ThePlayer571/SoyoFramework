using System.Collections.Generic;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEditor;

namespace SoyoFramework.OptionalKits.UIKit.Editor
{
    [CustomEditor(typeof(UISettings))]
    public class UISettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认编辑器
            DrawDefaultInspector();

            var uiSettings = (UISettings)target;

            // 检查 UIRoot 是否为空
            if (uiSettings.UIRoot == null)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("UIRoot 未设置！这是必需的配置项。", MessageType.Error);
                return; // UIRoot 为空时，后续验证没有意义，直接返回
            }

            // 验证LayerKey
            var layerKeys = uiSettings.LayerKeys;
            var pageConfigs = uiSettings.PageConfigs;

            if (layerKeys == null || pageConfigs == null)
                return;

            // 收集无效的LayerKey
            var invalidEntries = new List<string>();
            var layerKeySet = new HashSet<string>(layerKeys);

            foreach (var config in pageConfigs)
            {
                if (config == null)
                    continue;

                if (string.IsNullOrEmpty(config.LayerKey))
                {
                    invalidEntries.Add($"• Page '{config.PageName}':  LayerKey 为空");
                }
                else if (!layerKeySet.Contains(config. LayerKey))
                {
                    invalidEntries.Add($"• Page '{config.PageName}': LayerKey '{config. LayerKey}' 未在 LayerKeys 中注册");
                }
            }

            // 显示警告
            if (invalidEntries.Count > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox(
                    "发现以下无效的 LayerKey 配置：\n\n" + string.Join("\n", invalidEntries),
                    MessageType. Warning
                );
            }
        }
    }
}