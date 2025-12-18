using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEditor;
using UnityEngine;
using System.Reflection;

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
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(position, serializedValueProperty, new GUIContent(label.text), true);
                if (EditorGUI.EndChangeCheck())
                {
                    // 用反射将 _serializedValue 同步 到 Value（事件会触发）
                    object obj = property.GetValue();
                    if (obj != null)
                    {
                        var valueProp = obj.GetType().GetProperty("Value");
                        if (valueProp != null && valueProp.CanWrite)
                        {
                            valueProp.SetValue(obj, serializedValueProperty.GetGenericValue());
                        }
                    }
                }
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

    // ------- 工具扩展，便于通用取对象与泛型字段 --------
    public static class SerializedPropertyExtensions
    {
        // 获取目标实例对象
        public static object GetValue(this SerializedProperty property)
        {
            if (property == null) return null;
            object obj = property.serializedObject.targetObject;
            string[] path = property.propertyPath.Replace(".Array.data[", "[").Split('.');
            foreach (var pathItem in path)
            {
                if (pathItem.Contains("["))
                {
                    string fieldName = pathItem.Substring(0, pathItem.IndexOf("["));
                    int index = int.Parse(pathItem.Substring(pathItem.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetFieldOrProperty(obj, fieldName);
                    if (obj is System.Collections.IList list) obj = list[index];
                }
                else
                {
                    obj = GetFieldOrProperty(obj, pathItem);
                }
                if (obj == null) break;
            }
            return obj;
        }

        private static object GetFieldOrProperty(object obj, string name)
        {
            if (obj == null) return null;
            var type = obj.GetType();
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null) return field.GetValue(obj);
            var prop = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanRead) return prop.GetValue(obj, null);
            return null;
        }

        // 自动适配泛型类型、支持int/float/Vector3等
        public static object GetGenericValue(this SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer: return prop.intValue;
                case SerializedPropertyType.Boolean: return prop.boolValue;
                case SerializedPropertyType.Float:   return prop.floatValue;
                case SerializedPropertyType.String:  return prop.stringValue;
                case SerializedPropertyType.Vector2: return prop.vector2Value;
                case SerializedPropertyType.Vector3: return prop.vector3Value;
                case SerializedPropertyType.Color:   return prop.colorValue;
                case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue;
                case SerializedPropertyType.Enum: return prop.enumValueIndex;
                // 你可以继续扩展
                default: return null;
            }
        }
    }
}