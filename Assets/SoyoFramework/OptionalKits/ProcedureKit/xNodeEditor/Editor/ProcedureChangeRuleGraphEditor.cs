#if XNODE_SUPPORT

using System;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.ProcedureKit.Editor;
using SoyoFramework.OptionalKits.ProcedureKit.Editor.Window;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Editor
{
    [CustomNodeGraphEditor(typeof(ProcedureChangeRuleGraph))]
    public class ProcedureChangeRuleGraphEditor : NodeGraphEditor
    {
        private ProcedureChangeRuleGraph Graph => target as ProcedureChangeRuleGraph;

        public override void OnGUI()
        {
            base.OnGUI();

            DrawControlPanel();
            DrawReminderLabel();
        }

        public override void OnOpen()
        {
            // 如果没有配置，尝试加载默认配置
            if (Graph.AttachedConfig == null)
            {
                string savedPath = EditorPrefs.GetString(ProcedureKitEditorWindow.ConfigPathPrefKey, string.Empty);
                if (!string.IsNullOrEmpty(savedPath))
                {
                    var config = AssetDatabase.LoadAssetAtPath<ProcedureKitConfigSO>(savedPath);
                    if (config != null)
                    {
                        Undo.RecordObject(Graph, "Auto Assign Config");
                        Graph.AttachedConfig = config;
                        EditorUtility.SetDirty(Graph);
                        Debug.Log($"[ProcedureKit] Auto loaded config from: {savedPath}");
                    }
                }
            }

            // 如果有配置，读取并同步节点
            if (Graph.AttachedConfig != null)
            {
                Graph.ReadConfig();
            }
        }

        private void DrawControlPanel()
        {
            if (Graph == null) return;

            // 定义浮动面板的位置和大小
            Rect panelRect = new Rect(10, 10, 300, 100);

            // 开始浮动面板
            GUILayout.BeginArea(panelRect, GUI.skin.box);

            GUILayout.Label("Procedure Config Control", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            // 显示 ProcedureKitConfigSO 字段
            ProcedureKitConfigSO newConfig = (ProcedureKitConfigSO)EditorGUILayout.ObjectField(
                "Procedure Config",
                Graph.AttachedConfig,
                typeof(ProcedureKitConfigSO),
                false
            );

            // 如果字段值发生变化，更新并标记为脏数据
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Graph, "Change Attached Config");
                Graph.AttachedConfig = newConfig;
                EditorUtility.SetDirty(Graph);
            }

            // 如果 ProcedureKitConfigSO 有值，显示操作按钮
            if (Graph.AttachedConfig != null)
            {
                GUILayout.Space(5);

                // 按钮：创建/销毁节点以同步SO文件
                if (GUILayout.Button("创建/销毁节点 以同步SO文件"))
                {
                    Graph.ReadConfig();
                }

                // 按钮：写入SO文件
                if (GUILayout.Button("写入SO文件"))
                {
                    Graph.WriteConfig();
                }
            }

            GUILayout.EndArea();
        }

        private void DrawReminderLabel()
        {
            if (Graph == null || Graph.AttachedConfig == null) return;

            // 计算右上角位置
            float labelWidth = 200;
            float labelHeight = 30;
            Rect labelRect = new Rect(window.position.width - labelWidth - 10, 10, labelWidth, labelHeight);

            // 创建带背景的样式
            GUIStyle reminderStyle = new GUIStyle(GUI.skin.box);
            reminderStyle.alignment = TextAnchor.MiddleCenter;
            reminderStyle.normal.textColor = new Color(1f, 0.7f, 0f); // 橙色文字
            reminderStyle.fontStyle = FontStyle.Bold;
            reminderStyle.fontSize = 11; // 稍微调小字体

            GUI.Label(labelRect, "记得写入喵（关闭界面不会自动写入）", reminderStyle);
        }

        public override string GetNodeMenuName(Type type)
        {
            return null;
        }

        public override Node CopyNode(Node original)
        {
            "[ProcedureKit] 你不应该复制 ProcedureChangeRuleGraph 的节点，请使用左上角的菜单自动删除与添加节点".LogWarning();
            return null;
        }

        public override bool CanRemove(Node node)
        {
            "[ProcedureKit] 你不应该删除 ProcedureChangeRuleGraph 的节点，请使用左上角的菜单自动删除与添加节点".LogWarning();
            return false;
        }

        public override bool CanConnect(NodePort output, NodePort input)
        {
            if (output.node == input.node) return false;

            return base.CanConnect(output, input);
        }
    }
}
#endif