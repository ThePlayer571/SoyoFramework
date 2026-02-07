using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.ProcedureKit.Editor.Window
{
    public partial class ProcedureKitEditorWindow
    {
        private void DrawRegion2_AllowedPreviousProcedures()
        {
            // 顶部提示 - 不依赖xNode，只是文字提示
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox(
                "💡 提示：如果你导入了xNode，可以使用节点编辑器来可视化编辑 Allowed Previous，详情见Readme文档",
                MessageType.Info);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 原有的列表编辑界面（保持不变）
            for (int procIndex = 1; procIndex < _procedures.Count; procIndex++)
            {
                var proc = _procedures[procIndex];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // 标题行
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{proc.EnumValue}] {proc.Name}", EditorStyles.boldLabel,
                    GUILayout.Width(180));
                EditorGUILayout.LabelField("← Allowed Previous:", GUILayout.Width(120));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                // 已选择的 AllowedPrevious
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);

                var toRemove = new List<int>();
                foreach (var prevEnumValue in proc.AllowedPreviousEnumValues)
                {
                    var prevProc = _procedures.FirstOrDefault(p => p.EnumValue == prevEnumValue);
                    if (prevProc != null)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(140));
                        EditorGUILayout.LabelField($"[{prevProc.EnumValue}] {prevProc.Name}", GUILayout.Width(110));
                        if (GUILayout.Button("×", GUILayout.Width(18)))
                        {
                            toRemove.Add(prevEnumValue);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                foreach (var enumValue in toRemove)
                    proc.AllowedPreviousEnumValues.Remove(enumValue);

                // 添加按钮
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    ShowAddPreviousProcedureMenu(proc);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            // 如果只有 Entrance，显示提示
            if (_procedures.Count <= 1)
            {
                EditorGUILayout.HelpBox("目前只有 Entrance，请先在 Region 1 添加其他 Procedure", MessageType.Info);
            }
        }

        private void ShowAddPreviousProcedureMenu(ProcedureEntry targetProc)
        {
            var menu = new GenericMenu();
            foreach (var proc in _procedures)
            {
                if (!targetProc.AllowedPreviousEnumValues.Contains(proc.EnumValue) &&
                    proc.EnumValue != targetProc.EnumValue)
                {
                    int enumValue = proc.EnumValue;
                    menu.AddItem(new GUIContent($"[{proc.EnumValue}] {proc.Name}"), false,
                        () => { targetProc.AllowedPreviousEnumValues.Add(enumValue); });
                }
            }

            if (menu.GetItemCount() == 0)
                menu.AddDisabledItem(new GUIContent("(No more procedures to add)"));

            menu.ShowAsContext();
        }
    }
}