using System.Collections;
using System.Reflection;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    [CustomPropertyDrawer(typeof(BindableProperty<>), true)]
    public class BindablePropertyDrawer : PropertyDrawer
    {
        private const string SerializedValueFieldName = "_serializedValue";
        private const string ForceTriggerMethodName = "ForceTrigger";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var valueProperty = property.FindPropertyRelative(SerializedValueFieldName);

            // 特殊情况处理：无法序列化
            var findSuccessfully = valueProperty != null;
            if (!findSuccessfully)
            {
                EditorGUI.LabelField(position, label.text, "目标字段无法序列化");
                EditorGUI.EndProperty();
                return;
            }

            // 默认绘制
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, valueProperty, label, true);

            if (EditorGUI.EndChangeCheck() && property.serializedObject.ApplyModifiedProperties())
            {
                TriggerEvent(property);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(SerializedValueFieldName);

            var findSuccessfully = valueProperty != null;
            if (!findSuccessfully)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(valueProperty, label, true);
        }

        private static void TriggerEvent(SerializedProperty property)
        {
            var bindablePropertyObj = Utils.GetTargetObject(property);

            if (bindablePropertyObj == null) return;

            var forceTriggerMethod = bindablePropertyObj.GetType().GetMethod(ForceTriggerMethodName);
            if (forceTriggerMethod == null)
            {
                $"找不到 BindableProperty的{ForceTriggerMethodName} 方法".LogError();
                return;
            }

            forceTriggerMethod.Invoke(bindablePropertyObj, null);
        }
    }
}