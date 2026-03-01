using System;
using System.Reflection;
using SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Editor
{
    [CustomEditor(typeof(SoyoToggle), true)]
    public class SoyoToggleEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            // 第一行显示脚本名称（只读），效果类似 MonoBehaviour 默认 Inspector 的 Script: xxx
            DrawScriptField();

            base.OnInspectorGUI();

            SerializedObject so = serializedObject;
            Type type = target.GetType();
            // 只显示实际声明在本类及其子类的字段
            DrawCustomFields(so, type, typeof(Toggle));
        }

        private void DrawScriptField()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }

        private void DrawCustomFields(SerializedObject so, Type scriptType, Type baseLimit)
        {
            so.Update();
            while (scriptType != null && scriptType != baseLimit)
            {
                var fields = scriptType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                                  BindingFlags.DeclaredOnly);
                foreach (var field in fields)
                {
                    bool isPublic = field.IsPublic;
                    bool isSerialized = field.GetCustomAttribute<SerializeField>() != null;
                    if (isPublic || isSerialized)
                    {
                        var prop = so.FindProperty(field.Name);
                        if (prop != null)
                            EditorGUILayout.PropertyField(prop, true);
                    }
                }

                scriptType = scriptType.BaseType;
            }

            so.ApplyModifiedProperties();
        }
    }
}