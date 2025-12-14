using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    [CustomPropertyDrawer(typeof(BindableProperty<>), true)]
    public class BindablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取 _serializedValue 字段
            SerializedProperty serializedValueProperty = property.FindPropertyRelative("_serializedValue");

            if (serializedValueProperty != null)
            {
                // 使用 "Value" 作为显示标签，或者使用原有的 label
                EditorGUI.PropertyField(position, serializedValueProperty, new GUIContent(label.text), true);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "无法找到 _serializedValue");
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty serializedValueProperty = property.FindPropertyRelative("_serializedValue");

            if (serializedValueProperty != null)
            {
                return EditorGUI.GetPropertyHeight(serializedValueProperty, label, true);
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}