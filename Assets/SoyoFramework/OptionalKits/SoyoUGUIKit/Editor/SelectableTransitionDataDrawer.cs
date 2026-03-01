using SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.TransitionData;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Editor
{
    [CustomPropertyDrawer(typeof(SelectableTransitionData))]
    public class SelectableTransitionDataDrawer : PropertyDrawer
    {
        #region Serialized Property Names

        private const string kTransitionPath = "m_Transition";
        private const string kTargetGraphicPath = "m_TargetGraphic";
        private const string kColorsPath = "m_Colors";
        private const string kSpriteStatePath = "m_SpriteState";
        private const string kAnimationTriggersPath = "m_AnimationTriggers";

        #endregion

        #region Property Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f;

            // Foldout
            height += EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
                return height;

            height += EditorGUIUtility.standardVerticalSpacing;

            // Transition
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var transitionProp = property.FindPropertyRelative(kTransitionPath);
            var transition = (Selectable.Transition)transitionProp.enumValueIndex;

            // Target Graphic (仅 ColorTint 和 SpriteSwap 显示)
            if (transition == Selectable.Transition.ColorTint || transition == Selectable.Transition.SpriteSwap)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // 根据 Transition 类型计算额外高度
            switch (transition)
            {
                case Selectable.Transition.ColorTint:
                    height += GetColorBlockHeight(property.FindPropertyRelative(kColorsPath));
                    break;
                case Selectable.Transition.SpriteSwap:
                    height += GetSpriteStateHeight(property.FindPropertyRelative(kSpriteStatePath));
                    break;
                case Selectable.Transition.Animation:
                    height += GetAnimationTriggersHeight(property.FindPropertyRelative(kAnimationTriggersPath));
                    break;
            }

            return height;
        }

        private float GetColorBlockHeight(SerializedProperty colorsProp)
        {
            // ColorBlock: normalColor, highlightedColor, pressedColor, selectedColor, disabledColor, colorMultiplier, fadeDuration
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 7;
        }

        private float GetSpriteStateHeight(SerializedProperty spriteStateProp)
        {
            // SpriteState: highlightedSprite, pressedSprite, selectedSprite, disabledSprite
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;
        }

        private float GetAnimationTriggersHeight(SerializedProperty animTriggersProp)
        {
            // AnimationTriggers: normalTrigger, highlightedTrigger, pressedTrigger, selectedTrigger, disabledTrigger
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5;
        }

        #endregion

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Foldout
            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel++;

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Transition
            var transitionProp = property.FindPropertyRelative(kTransitionPath);
            y = DrawProperty(position, y, transitionProp, new GUIContent("Transition"));

            var transition = (Selectable.Transition)transitionProp.enumValueIndex;

            // Target Graphic (仅 ColorTint 和 SpriteSwap 显示)
            if (transition == Selectable.Transition.ColorTint || transition == Selectable.Transition.SpriteSwap)
            {
                var targetGraphicProp = property.FindPropertyRelative(kTargetGraphicPath);
                y = DrawProperty(position, y, targetGraphicProp, new GUIContent("Target Graphic"));
            }

            // 根据 Transition 类型绘制对应配置
            switch (transition)
            {
                case Selectable.Transition.ColorTint:
                    DrawColorBlock(position, ref y, property.FindPropertyRelative(kColorsPath));
                    break;
                case Selectable.Transition.SpriteSwap:
                    DrawSpriteState(position, ref y, property.FindPropertyRelative(kSpriteStatePath));
                    break;
                case Selectable.Transition.Animation:
                    DrawAnimationTriggers(position, ref y, property.FindPropertyRelative(kAnimationTriggersPath));
                    break;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        #endregion

        #region Draw Methods

        private float DrawProperty(Rect position, float y, SerializedProperty prop, GUIContent label)
        {
            Rect rect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rect, prop, label);
            return y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void DrawColorBlock(Rect position, ref float y, SerializedProperty colorsProp)
        {
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_NormalColor"),
                new GUIContent("Normal Color"));
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_HighlightedColor"),
                new GUIContent("Highlighted Color"));
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_PressedColor"),
                new GUIContent("Pressed Color"));
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_SelectedColor"),
                new GUIContent("Selected Color"));
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_DisabledColor"),
                new GUIContent("Disabled Color"));
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_ColorMultiplier"),
                new GUIContent("Color Multiplier"));
            y = DrawProperty(position, y, colorsProp.FindPropertyRelative("m_FadeDuration"),
                new GUIContent("Fade Duration"));
        }

        private void DrawSpriteState(Rect position, ref float y, SerializedProperty spriteStateProp)
        {
            y = DrawProperty(position, y, spriteStateProp.FindPropertyRelative("m_HighlightedSprite"),
                new GUIContent("Highlighted Sprite"));
            y = DrawProperty(position, y, spriteStateProp.FindPropertyRelative("m_PressedSprite"),
                new GUIContent("Pressed Sprite"));
            y = DrawProperty(position, y, spriteStateProp.FindPropertyRelative("m_SelectedSprite"),
                new GUIContent("Selected Sprite"));
            y = DrawProperty(position, y, spriteStateProp.FindPropertyRelative("m_DisabledSprite"),
                new GUIContent("Disabled Sprite"));
        }

        private void DrawAnimationTriggers(Rect position, ref float y, SerializedProperty animTriggersProp)
        {
            y = DrawProperty(position, y, animTriggersProp.FindPropertyRelative("m_NormalTrigger"),
                new GUIContent("Normal Trigger"));
            y = DrawProperty(position, y, animTriggersProp.FindPropertyRelative("m_HighlightedTrigger"),
                new GUIContent("Highlighted Trigger"));
            y = DrawProperty(position, y, animTriggersProp.FindPropertyRelative("m_PressedTrigger"),
                new GUIContent("Pressed Trigger"));
            y = DrawProperty(position, y, animTriggersProp.FindPropertyRelative("m_SelectedTrigger"),
                new GUIContent("Selected Trigger"));
            y = DrawProperty(position, y, animTriggersProp.FindPropertyRelative("m_DisabledTrigger"),
                new GUIContent("Disabled Trigger"));
        }

        #endregion
    }
}