using System;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    /// <summary>
    /// CustomEnumPopup 特性的 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomEnumPopupAttribute))]
    public class CustomEnumPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 验证属性类型
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.HelpBox(position, "CustomEnumPopup 只能用于枚举类型", MessageType.Error);
                return;
            }

            CustomEnumPopupAttribute attr = (CustomEnumPopupAttribute)attribute;

            // 获取显示名称
            string displayName = string.IsNullOrEmpty(attr.DisplayName) ? label.text : attr.DisplayName;

            // 获取当前枚举值的显示名称
            string[] enumNames = property.enumDisplayNames;
            int currentIndex = property.enumValueIndex;
            string currentValueName = currentIndex >= 0 && currentIndex < enumNames.Length
                ? enumNames[currentIndex]
                : "Unknown";

            // 获取枚举的实际值用于显示
            Type enumType = fieldInfo.FieldType;
            Array enumValues = Enum.GetValues(enumType);
            int currentEnumValue = currentIndex >= 0 && currentIndex < enumValues.Length
                ? (int)enumValues.GetValue(currentIndex)
                : 0;

            // 绘制标签和按钮
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2, position.y,
                position.width - EditorGUIUtility.labelWidth - 2, position.height);

            EditorGUI.LabelField(labelRect, displayName);

            // 按钮显示当前值和对应的整数值
            string buttonText = $"{currentValueName} ({currentEnumValue})";

            if (GUI.Button(buttonRect, buttonText, EditorStyles.popup))
            {
                // 打开自定义弹窗
                CustomEnumPopupWindow.Show(buttonRect, property, enumType, attr.SortType);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}