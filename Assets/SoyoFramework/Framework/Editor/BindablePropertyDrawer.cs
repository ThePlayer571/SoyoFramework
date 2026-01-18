using System.Reflection;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    [CustomPropertyDrawer(typeof(BindableProperty<>), true)]
    public class BindablePropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var serializedValue = property.FindPropertyRelative("_serializedValue");

            if (serializedValue == null)
            {
                EditorGUI.LabelField(position, label.text, "无法找到 _serializedValue");
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.BeginChangeCheck();

            // 根据类型选择绘制方式
            if (serializedValue.hasVisibleChildren)
            {
                DrawExpandableProperty(position, serializedValue, label);
            }
            else
            {
                EditorGUI.PropertyField(position, serializedValue, label, true);
            }

            if (EditorGUI.EndChangeCheck())
            {
                // 应用修改
                property.serializedObject.ApplyModifiedProperties();

                // 同步值并触发事件
                SyncAndTrigger(property);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var serializedValue = property.FindPropertyRelative("_serializedValue");

            if (serializedValue == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            if (serializedValue.hasVisibleChildren)
            {
                return GetExpandablePropertyHeight(serializedValue);
            }

            return EditorGUI.GetPropertyHeight(serializedValue, label, true);
        }

        #region 绘制方法

        private void DrawExpandableProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            float y = position.y;

            // Foldout
            var foldoutRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (!property.isExpanded) return;

            y += EditorGUIUtility.singleLineHeight + Spacing;
            EditorGUI.indentLevel++;

            // 绘制子字段
            foreach (var child in IterateVisibleChildren(property))
            {
                float h = EditorGUI.GetPropertyHeight(child, true);
                var rect = new Rect(position.x, y, position.width, h);
                EditorGUI.PropertyField(rect, child, true);
                y += h + Spacing;
            }

            EditorGUI.indentLevel--;
        }

        private float GetExpandablePropertyHeight(SerializedProperty property)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                foreach (var child in IterateVisibleChildren(property))
                {
                    height += EditorGUI.GetPropertyHeight(child, true) + Spacing;
                }
            }

            return height;
        }

        #endregion

        #region 核心逻辑

        private void SyncAndTrigger(SerializedProperty property)
        {
            object target = GetTargetObject(property);
            if (target == null) return;

            var type = target.GetType();
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            // 获取 _serializedValue
            var serializedField = type.GetField("_serializedValue", flags);
            if (serializedField == null) return;

            object newValue = serializedField.GetValue(target);

            // 调用 SetValueWithoutTrigger
            var setMethod = type.GetMethod("SetValueWithoutTrigger", flags);
            setMethod?.Invoke(target, new[] { newValue });

            // 调用 ForceTrigger
            var triggerMethod = type.GetMethod("ForceTrigger", flags);
            triggerMethod?.Invoke(target, null);
        }

        #endregion

        #region 工具方法

        private static object GetTargetObject(SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");

            foreach (string element in path.Split('.'))
            {
                if (obj == null) return null;

                if (element.Contains("["))
                {
                    int bracketIndex = element.IndexOf("[");
                    string fieldName = element.Substring(0, bracketIndex);
                    int index = int.Parse(element.Substring(bracketIndex + 1).TrimEnd(']'));

                    obj = GetFieldValue(obj, fieldName);

                    if (obj is System.Collections.IList list && index < list.Count)
                    {
                        obj = list[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    obj = GetFieldValue(obj, element);
                }
            }

            return obj;
        }

        private static object GetFieldValue(object obj, string fieldName)
        {
            if (obj == null) return null;

            var type = obj.GetType();
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            while (type != null)
            {
                var field = type.GetField(fieldName, flags);
                if (field != null) return field.GetValue(obj);

                var prop = type.GetProperty(fieldName, flags);
                if (prop != null && prop.CanRead) return prop.GetValue(obj);

                type = type.BaseType;
            }

            return null;
        }

        private static System.Collections.Generic.IEnumerable<SerializedProperty> IterateVisibleChildren(
            SerializedProperty property)
        {
            var current = property.Copy();
            var end = property.Copy();
            end.NextVisible(false);

            if (current.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(current, end)) break;
                    yield return current.Copy();
                } while (current.NextVisible(false));
            }
        }

        #endregion
    }
}