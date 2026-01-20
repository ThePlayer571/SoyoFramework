using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.ProcedureKit.Editor.Window
{
    public partial class ProcedureKitEditorWindow
    {
        private void DrawRegion1_ProcedureIdAndTags()
        {
            // 顶部按钮区域
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("按 Enum 值排序", GUILayout.Width(120)))
            {
                SortByEnumValue();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                int nextEnumValue = _tags.Count > 0 ? _tags.Max(t => t.EnumValue) + 1 : 0;
                _tags.Add(new TagEntry { Name = "NewTag", EnumValue = nextEnumValue });
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 表格头部 - Tag 的 EnumValue 行
            EditorGUILayout. BeginHorizontal();
            GUILayout.Space(50); // EnumValue 列占位
            GUILayout.Space(120); // ProcedureId 列占位
            GUILayout.Space(22); // 删除按钮占位

            for (int i = 0; i < _tags.Count; i++)
            {
                bool isEnumValueError = IsTagEnumValueError(_tags[i].EnumValue);
                if (isEnumValueError)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    _tags[i].EnumValue = EditorGUILayout.IntField(_tags[i].EnumValue, GUILayout.Width(100));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    _tags[i].EnumValue = EditorGUILayout.IntField(_tags[i].EnumValue, GUILayout.Width(100));
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // 表格头部 - Tag 名称行
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("", GUILayout.Width(50)); // EnumValue 列占位
            EditorGUILayout.LabelField("ProcedureId", EditorStyles.boldLabel, GUILayout.Width(120));
            GUILayout.Space(22); // 删除按钮占位

            for (int i = 0; i < _tags.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(100));

                bool isErrorTag = IsTagNameError(_tags[i].Name);
                if (isErrorTag)
                {
                    var originalColor = GUI. backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    _tags[i].Name = EditorGUILayout.TextField(_tags[i].Name, GUILayout.Width(75));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    _tags[i].Name = EditorGUILayout.TextField(_tags[i].Name, GUILayout. Width(75));
                }

                if (GUILayout.Button("×", GUILayout.Width(18)))
                {
                    RemoveTag(i);
                    i--;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout. EndHorizontal();

            // 表格内容
            for (int p = 0; p < _procedures.Count; p++)
            {
                var proc = _procedures[p];
                EditorGUILayout.BeginHorizontal();

                bool isEntrance = p == 0 && proc.Name == "Entrance";

                // EnumValue 输入框
                GUI.enabled = !isEntrance;
                bool isEnumValueError = IsProcedureEnumValueError(proc.EnumValue);
                if (isEnumValueError)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    proc.EnumValue = EditorGUILayout.IntField(proc.EnumValue, GUILayout.Width(50));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    proc.EnumValue = EditorGUILayout.IntField(proc. EnumValue, GUILayout. Width(50));
                }

                GUI.enabled = true;

                // ProcedureId 名称
                GUI.enabled = !isEntrance;
                bool isErrorProcedure = IsProcedureNameError(proc.Name);
                if (isErrorProcedure)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    proc.Name = EditorGUILayout.TextField(proc.Name, GUILayout. Width(120));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    proc.Name = EditorGUILayout.TextField(proc.Name, GUILayout.Width(120));
                }

                // 删除按钮
                if (! isEntrance && GUILayout. Button("×", GUILayout. Width(18)))
                {
                    RemoveProcedure(p);
                    p--;
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                else if (isEntrance)
                {
                    GUILayout.Space(22);
                }

                GUI.enabled = true;

                // Tags 勾选
                for (int t = 0; t < _tags.Count; t++)
                {
                    bool hasTag = proc.TagEnumValues.Contains(_tags[t].EnumValue);
                    var style = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 14,
                        alignment = TextAnchor.MiddleCenter
                    };

                    string buttonText = hasTag ? "✔" : "";
                    if (GUILayout.Button(buttonText, style, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (hasTag)
                            proc.TagEnumValues.Remove(_tags[t].EnumValue);
                        else
                            proc.TagEnumValues.Add(_tags[t].EnumValue);
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            // 添加Procedure按钮
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout. Button("+", GUILayout.Width(25)))
            {
                int nextEnumValue = _procedures.Count > 0 ? _procedures.Max(p => p. EnumValue) + 1 : 0;
                _procedures.Add(new ProcedureEntry { Name = "NewProcedure", EnumValue = nextEnumValue });
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void SortByEnumValue()
        {
            // 保持 Entrance 在第一位，其余按 EnumValue 排序
            if (_procedures.Count > 1)
            {
                var entrance = _procedures[0];
                var others = _procedures.Skip(1).OrderBy(p => p.EnumValue).ToList();
                _procedures. Clear();
                _procedures.Add(entrance);
                _procedures. AddRange(others);
            }

            // Tag 按 EnumValue 排序
            _tags = _tags.OrderBy(t => t.EnumValue).ToList();
        }

        private void RemoveTag(int index)
        {
            int removedEnumValue = _tags[index]. EnumValue;
            _tags.RemoveAt(index);

            // 从所有 procedure 中移除对该 Tag 的引用
            foreach (var proc in _procedures)
            {
                proc.TagEnumValues.Remove(removedEnumValue);
            }
        }

        private void RemoveProcedure(int index)
        {
            int removedEnumValue = _procedures[index]. EnumValue;
            _procedures.RemoveAt(index);

            // 从所有 procedure 的 AllowedPreviousEnumValues 中移除对该 Procedure 的引用
            foreach (var proc in _procedures)
            {
                proc.AllowedPreviousEnumValues.Remove(removedEnumValue);
            }
        }
    }
}