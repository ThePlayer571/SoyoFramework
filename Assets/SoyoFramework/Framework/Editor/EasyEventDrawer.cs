using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    /// <summary>
    /// 无参数 EasyEvent 的 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(EasyEvent), true)]
    public class EasyEventDrawer : PropertyDrawer
    {
        protected const float ButtonWidth = 60f;
        protected const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect labelRect = new Rect(position.x, position.y, position.width - ButtonWidth - 5f,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, "Trigger"))
            {
                EasyEvent easyEvent = GetTargetObjectOfProperty<EasyEvent>(property);
                if (easyEvent != null)
                {
                    easyEvent.EditorTrigger();
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected static T GetTargetObjectOfProperty<T>(SerializedProperty property) where T : class
        {
            return GetTargetObjectOfProperty(property) as T;
        }

        protected static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(". Array. data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (string element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = int.Parse(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null) return null;

            var type = source.GetType();
            while (type != null)
            {
                var field = type.GetField(name,
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);
                if (field != null)
                    return field.GetValue(source);

                var prop = type.GetProperty(name,
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                if (prop != null)
                    return prop.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;

            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current;
        }

        protected void TriggerGenericEvent(SerializedProperty property, string methodName)
        {
            object targetObject = GetTargetObjectOfProperty(property);
            if (targetObject == null) return;

            var type = targetObject.GetType();
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);

            if (method != null)
            {
                method.Invoke(targetObject, null);
            }
        }
    }

    /// <summary>
    /// 单参数 EasyEvent&lt;T&gt; 的 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(EasyEvent<>), true)]
    public class EasyEventT1Drawer : EasyEventDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty serializedValue = property.FindPropertyRelative("_serializedValue");

            if (serializedValue != null)
            {
                // 判断是否为复杂类型（可展开）
                bool isExpandable = serializedValue.hasVisibleChildren;

                if (isExpandable)
                {
                    // 复杂类型：标签行 + 按钮，子字段缩进显示
                    DrawExpandableField(position, property, serializedValue, label);
                }
                else
                {
                    // 简单类型：单行显示
                    DrawSimpleField(position, property, serializedValue, label);
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "无法找到 _serializedValue");
            }

            EditorGUI.EndProperty();
        }

        private void DrawSimpleField(Rect position, SerializedProperty property, SerializedProperty serializedValue,
            GUIContent label)
        {
            float fieldWidth = position.width - ButtonWidth - 5f;
            Rect fieldRect = new Rect(position.x, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldRect, serializedValue, new GUIContent(label.text), false);

            if (GUI.Button(buttonRect, "Trigger"))
            {
                property.serializedObject.ApplyModifiedProperties();
                TriggerGenericEvent(property, "EditorTrigger");
            }
        }

        private void DrawExpandableField(Rect position, SerializedProperty property, SerializedProperty serializedValue,
            GUIContent label)
        {
            float currentY = position.y;

            // 第一行：Foldout + 按钮
            Rect foldoutRect = new Rect(position.x, currentY, position.width - ButtonWidth - 5f,
                EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.xMax - ButtonWidth, currentY, ButtonWidth,
                EditorGUIUtility.singleLineHeight);

            serializedValue.isExpanded = EditorGUI.Foldout(foldoutRect, serializedValue.isExpanded, label, true);

            if (GUI.Button(buttonRect, "Trigger"))
            {
                property.serializedObject.ApplyModifiedProperties();
                TriggerGenericEvent(property, "EditorTrigger");
            }

            // 如果展开，绘制子字段
            if (serializedValue.isExpanded)
            {
                currentY += EditorGUIUtility.singleLineHeight + Spacing;
                EditorGUI.indentLevel++;

                foreach (SerializedProperty childProperty in GetVisibleChildren(serializedValue))
                {
                    float childHeight = EditorGUI.GetPropertyHeight(childProperty, true);
                    Rect childRect = new Rect(position.x, currentY, position.width, childHeight);
                    EditorGUI.PropertyField(childRect, childProperty, true);
                    currentY += childHeight + Spacing;
                }

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty serializedValue = property.FindPropertyRelative("_serializedValue");

            if (serializedValue != null)
            {
                if (serializedValue.hasVisibleChildren)
                {
                    // 复杂类型
                    float height = EditorGUIUtility.singleLineHeight; // Foldout 行

                    if (serializedValue.isExpanded)
                    {
                        foreach (SerializedProperty childProperty in GetVisibleChildren(serializedValue))
                        {
                            height += EditorGUI.GetPropertyHeight(childProperty, true) + Spacing;
                        }
                    }

                    return height;
                }
                else
                {
                    // 简单类型
                    return EditorGUIUtility.singleLineHeight;
                }
            }

            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// 获取可见的子属性
        /// </summary>
        protected static System.Collections.Generic.IEnumerable<SerializedProperty> GetVisibleChildren(
            SerializedProperty property)
        {
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextSiblingProperty = property.Copy();
            nextSiblingProperty.NextVisible(false);

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty.Copy();
                } while (currentProperty.NextVisible(false));
            }
        }
    }

    /// <summary>
    /// 双参数 EasyEvent&lt;T1, T2&gt; 的 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(EasyEvent<,>), true)]
    public class EasyEventT2Drawer : EasyEventT1Drawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty serializedValue1 = property.FindPropertyRelative("_serializedValue1");
            SerializedProperty serializedValue2 = property.FindPropertyRelative("_serializedValue2");

            if (serializedValue1 != null && serializedValue2 != null)
            {
                float currentY = position.y;

                // 标签行和按钮
                Rect labelRect = new Rect(position.x, currentY, position.width - ButtonWidth - 5f,
                    EditorGUIUtility.singleLineHeight);
                Rect buttonRect = new Rect(position.xMax - ButtonWidth, currentY, ButtonWidth,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(labelRect, label);
                if (GUI.Button(buttonRect, "Trigger"))
                {
                    property.serializedObject.ApplyModifiedProperties();
                    TriggerGenericEvent(property, "EditorTrigger");
                }

                currentY += EditorGUIUtility.singleLineHeight + Spacing;

                EditorGUI.indentLevel++;

                // Arg 1
                currentY = DrawArgumentField(position, serializedValue1, "Arg 1", currentY);

                // Arg 2
                currentY = DrawArgumentField(position, serializedValue2, "Arg 2", currentY);

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "无法找到序列化字段");
            }

            EditorGUI.EndProperty();
        }

        protected float DrawArgumentField(Rect position, SerializedProperty serializedValue, string argLabel,
            float currentY)
        {
            if (serializedValue.hasVisibleChildren)
            {
                // 复杂类型：Foldout
                Rect foldoutRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);
                serializedValue.isExpanded = EditorGUI.Foldout(foldoutRect, serializedValue.isExpanded,
                    new GUIContent(argLabel), true);
                currentY += EditorGUIUtility.singleLineHeight + Spacing;

                if (serializedValue.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    foreach (SerializedProperty childProperty in GetVisibleChildren(serializedValue))
                    {
                        float childHeight = EditorGUI.GetPropertyHeight(childProperty, true);
                        Rect childRect = new Rect(position.x, currentY, position.width, childHeight);
                        EditorGUI.PropertyField(childRect, childProperty, true);
                        currentY += childHeight + Spacing;
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                // 简单类型：单行
                float height = EditorGUI.GetPropertyHeight(serializedValue, true);
                Rect rect = new Rect(position.x, currentY, position.width, height);
                EditorGUI.PropertyField(rect, serializedValue, new GUIContent(argLabel), true);
                currentY += height + Spacing;
            }

            return currentY;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty serializedValue1 = property.FindPropertyRelative("_serializedValue1");
            SerializedProperty serializedValue2 = property.FindPropertyRelative("_serializedValue2");

            if (serializedValue1 != null && serializedValue2 != null)
            {
                float height = EditorGUIUtility.singleLineHeight + Spacing; // 标签行
                height += GetArgumentFieldHeight(serializedValue1);
                height += GetArgumentFieldHeight(serializedValue2);
                return height;
            }

            return EditorGUIUtility.singleLineHeight;
        }

        protected float GetArgumentFieldHeight(SerializedProperty serializedValue)
        {
            if (serializedValue.hasVisibleChildren)
            {
                float height = EditorGUIUtility.singleLineHeight + Spacing; // Foldout 行

                if (serializedValue.isExpanded)
                {
                    foreach (SerializedProperty childProperty in GetVisibleChildren(serializedValue))
                    {
                        height += EditorGUI.GetPropertyHeight(childProperty, true) + Spacing;
                    }
                }

                return height;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(serializedValue, true) + Spacing;
            }
        }
    }

    /// <summary>
    /// 三参数 EasyEvent&lt;T1, T2, T3&gt; 的 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(EasyEvent<,,>), true)]
    public class EasyEventT3Drawer : EasyEventT2Drawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty serializedValue1 = property.FindPropertyRelative("_serializedValue1");
            SerializedProperty serializedValue2 = property.FindPropertyRelative("_serializedValue2");
            SerializedProperty serializedValue3 = property.FindPropertyRelative("_serializedValue3");

            if (serializedValue1 != null && serializedValue2 != null && serializedValue3 != null)
            {
                float currentY = position.y;

                // 标签行和按钮
                Rect labelRect = new Rect(position.x, currentY, position.width - ButtonWidth - 5f,
                    EditorGUIUtility.singleLineHeight);
                Rect buttonRect = new Rect(position.xMax - ButtonWidth, currentY, ButtonWidth,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(labelRect, label);
                if (GUI.Button(buttonRect, "Trigger"))
                {
                    property.serializedObject.ApplyModifiedProperties();
                    TriggerGenericEvent(property, "EditorTrigger");
                }

                currentY += EditorGUIUtility.singleLineHeight + Spacing;

                EditorGUI.indentLevel++;

                currentY = DrawArgumentField(position, serializedValue1, "Arg 1", currentY);
                currentY = DrawArgumentField(position, serializedValue2, "Arg 2", currentY);
                currentY = DrawArgumentField(position, serializedValue3, "Arg 3", currentY);

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "无法找到序列化字段");
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty serializedValue1 = property.FindPropertyRelative("_serializedValue1");
            SerializedProperty serializedValue2 = property.FindPropertyRelative("_serializedValue2");
            SerializedProperty serializedValue3 = property.FindPropertyRelative("_serializedValue3");

            if (serializedValue1 != null && serializedValue2 != null && serializedValue3 != null)
            {
                float height = EditorGUIUtility.singleLineHeight + Spacing;
                height += GetArgumentFieldHeight(serializedValue1);
                height += GetArgumentFieldHeight(serializedValue2);
                height += GetArgumentFieldHeight(serializedValue3);
                return height;
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}