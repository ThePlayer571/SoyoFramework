using SoyoFramework.ToolKits.Runtime.UGUIKit;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace SoyoFramework.ToolKits.Editor.UGUIKit
{
    [CustomEditor(typeof(BetterToggle), true)]
    [CanEditMultipleObjects]
    public class BetterToggleEditor : ToggleEditor
    {
        private SerializedProperty _isOnColor;
        private SerializedProperty _isOnSprite;
        private SerializedProperty _isOnTrigger;

        protected override void OnEnable()
        {
            base.OnEnable();
            _isOnColor = serializedObject.FindProperty("isOnColor");
            _isOnSprite = serializedObject.FindProperty("isOnSprite");
            _isOnTrigger = serializedObject.FindProperty("isOnTrigger");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("IsOn Config", EditorStyles.boldLabel);

            BetterToggle betterToggle = target as BetterToggle;
            if (betterToggle != null)
            {
                switch (betterToggle.transition)
                {
                    case Selectable.Transition.ColorTint:
                        EditorGUILayout.PropertyField(_isOnColor, new UnityEngine.GUIContent("IsOn Color"));
                        break;
                    case Selectable.Transition.SpriteSwap:
                        EditorGUILayout.PropertyField(_isOnSprite, new UnityEngine.GUIContent("IsOn Sprite"));
                        break;
                    case Selectable.Transition.Animation:
                        EditorGUILayout.PropertyField(_isOnTrigger, new UnityEngine.GUIContent("IsOn Trigger"));
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}