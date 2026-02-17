using System;
using System.Collections.Generic;
using System.Reflection;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    public abstract class EasyEventDrawerBase : PropertyDrawer
    {
        private const float ButtonWidth = 60f;
        private const string ButtonTriggerLabel = "Trigger";
        protected const string Arg1FieldName = "_arg1";
        protected const string Arg2FieldName = "_arg2";
        protected const string Arg3FieldName = "_arg3";

        private static MethodInfo GetTriggerMethodInfo(object targetObject)
        {
            const string triggerMethodName = "EditorTrigger";

            if (targetObject == null) return null;

            var type = targetObject.GetType();
            var method = type.GetMethod(triggerMethodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (method == null)
            {
                "找不到 EditorTrigger 方法，无法触发 EasyEvent".LogError();
                return null;
            }

            return method;
        }

        protected static void DrawTriggerButton(float xMax, float y, SerializedProperty property)
        {
            var buttonRect = new Rect(xMax - ButtonWidth, y, ButtonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, ButtonTriggerLabel))
            {
                var easyEventObj = Utils.GetTargetObject(property);
                var triggerMethod = GetTriggerMethodInfo(easyEventObj);
                try
                {
                    triggerMethod?.Invoke(easyEventObj, null);
                }
                catch (Exception e)
                {
                    $"触发事件时发生异常: {e}".LogError();
                }

                EditorApplication.QueuePlayerLoopUpdate();
            }
        }

        protected static void DrawArg(Rect rect, SerializedProperty property, string label)
        {
            EditorGUI.indentLevel++;

            var drawable = property != null;

            if (drawable)
            {
                EditorGUI.PropertyField(rect, property, new GUIContent(label), true);
            }
            else
            {
                rect = EditorGUI.PrefixLabel(rect, new GUIContent(label));
                EditorGUI.LabelField(rect, "目标字段无法序列化");
            }

            EditorGUI.indentLevel--;
        }

        protected static IEnumerable<SerializedProperty> GetVisibleChildren(SerializedProperty property)
        {
            if (property == null || !property.hasVisibleChildren) yield break;

            var currentProperty = property.Copy();
            var endMark = property.Copy();
            endMark.NextVisible(false);

            // 进入子属性
            if (!currentProperty.NextVisible(true)) yield break;

            do
            {
                if (SerializedProperty.EqualContents(currentProperty, endMark))
                    break;

                yield return currentProperty.Copy();
            } while (currentProperty.NextVisible(false));
        }
    }

    [CustomPropertyDrawer(typeof(EasyEvent), true)]
    public class EasyEventDrawer : EasyEventDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            DrawTriggerButton(position.xMax, position.y, property);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(EasyEvent<>), true)]
    public class EasyEventT1Drawer : EasyEventDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label);
            DrawTriggerButton(position.xMax, position.y, property);

            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            // 绘制Args
            var arg1 = property.FindPropertyRelative(Arg1FieldName);

            DrawArg(position, arg1, "Arg");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var argProperty = property.FindPropertyRelative(Arg1FieldName);

            var height = EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.standardVerticalSpacing
                      + (argProperty == null
                          ? EditorGUIUtility.singleLineHeight
                          : EditorGUI.GetPropertyHeight(argProperty, label, true));

            return height;
        }
    }

    [CustomPropertyDrawer(typeof(EasyEvent<,>), true)]
    public class EasyEventT2Drawer : EasyEventDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label);
            DrawTriggerButton(position.xMax, position.y, property);

            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // 绘制Args
            var arg1 = property.FindPropertyRelative(Arg1FieldName);
            var arg2 = property.FindPropertyRelative(Arg2FieldName);
            var arg1Height = EditorGUI.GetPropertyHeight(arg1, GUIContent.none, true);
            var arg2Height = EditorGUI.GetPropertyHeight(arg2, GUIContent.none, true);
            var arg1Rect = new Rect(position.x, position.y, position.width, arg1Height);
            var arg2Rect = new Rect(position.x, position.y + arg1Height + EditorGUIUtility.standardVerticalSpacing,
                position.width, arg2Height);

            DrawArg(arg1Rect, arg1, "Arg1");
            DrawArg(arg2Rect, arg2, "Arg2");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var arg1Property = property.FindPropertyRelative(Arg1FieldName);
            var arg2Property = property.FindPropertyRelative(Arg2FieldName);

            var height = EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.standardVerticalSpacing
                      + (arg1Property == null
                          ? EditorGUIUtility.singleLineHeight
                          : EditorGUI.GetPropertyHeight(arg1Property, label, true));
            height += EditorGUIUtility.standardVerticalSpacing
                      + (arg2Property == null
                          ? EditorGUIUtility.singleLineHeight
                          : EditorGUI.GetPropertyHeight(arg2Property, label, true));

            return height;
        }
    }

    [CustomPropertyDrawer(typeof(EasyEvent<,,>), true)]
    public class EasyEventT3Drawer : EasyEventDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label);
            DrawTriggerButton(position.xMax, position.y, property);

            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // 绘制Args
            var arg1 = property.FindPropertyRelative(Arg1FieldName);
            var arg2 = property.FindPropertyRelative(Arg2FieldName);
            var arg3 = property.FindPropertyRelative(Arg3FieldName);

            var arg1Height = EditorGUI.GetPropertyHeight(arg1, GUIContent.none, true);
            var arg2Height = EditorGUI.GetPropertyHeight(arg2, GUIContent.none, true);
            var arg3Height = EditorGUI.GetPropertyHeight(arg3, GUIContent.none, true);

            var arg1Rect = new Rect(position.x, position.y, position.width, arg1Height);
            var arg2Rect = new Rect(position.x, position.y + arg1Height + EditorGUIUtility.standardVerticalSpacing,
                position.width, arg2Height);
            var arg3Rect = new Rect(position.x,
                position.y + arg1Height + EditorGUIUtility.standardVerticalSpacing + arg2Height +
                EditorGUIUtility.standardVerticalSpacing,
                position.width, arg3Height);

            DrawArg(arg1Rect, arg1, "Arg1");
            DrawArg(arg2Rect, arg2, "Arg2");
            DrawArg(arg3Rect, arg3, "Arg3");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var arg1Property = property.FindPropertyRelative(Arg1FieldName);
            var arg2Property = property.FindPropertyRelative(Arg2FieldName);
            var arg3Property = property.FindPropertyRelative(Arg3FieldName);

            var height = EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.standardVerticalSpacing
                      + (arg1Property == null
                          ? EditorGUIUtility.singleLineHeight
                          : EditorGUI.GetPropertyHeight(arg1Property, label, true));
            height += EditorGUIUtility.standardVerticalSpacing
                      + (arg2Property == null
                          ? EditorGUIUtility.singleLineHeight
                          : EditorGUI.GetPropertyHeight(arg2Property, label, true));
            height += EditorGUIUtility.standardVerticalSpacing
                      + (arg3Property == null
                          ? EditorGUIUtility.singleLineHeight
                          : EditorGUI.GetPropertyHeight(arg3Property, label, true));

            return height;
        }
    }
}