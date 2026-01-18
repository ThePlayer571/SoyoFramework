using SoyoFramework.ToolKits.Runtime.UGUIKit;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.ToolKits.Editor.UGUIKit
{
    [CustomEditor(typeof(Toggle), false)]
    [CanEditMultipleObjects]
    public class ToggleUpgradeEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            // 检查 target 是否有效
            if (target == null || serializedObject.targetObject == null)
                return;

            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("转换为 BetterToggle", GUILayout.Width(200), GUILayout.Height(30)))
            {
                // 延迟执行转换，避免在 GUI 绘制期间修改对象
                EditorApplication.delayCall += ConvertToBetterToggle;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
        }

        private void ConvertToBetterToggle()
        {
            // 收集需要转换的对象
            var gameObjectsToSelect = new System.Collections.Generic.List<GameObject>();
            var togglesToConvert = new System.Collections.Generic.List<Toggle>();

            foreach (var t in targets)
            {
                Toggle toggle = t as Toggle;
                if (toggle != null && toggle.GetType() == typeof(Toggle))
                {
                    togglesToConvert.Add(toggle);
                    gameObjectsToSelect.Add(toggle.gameObject);
                }
            }

            // 先清除选择，避免 Editor 访问已销毁的对象
            Selection.activeObject = null;

            foreach (var toggle in togglesToConvert)
            {
                ConvertSingleToggle(toggle);
            }

            // 转换完成后重新选择对象
            if (gameObjectsToSelect.Count > 0)
            {
                EditorApplication.delayCall += () => { Selection.objects = gameObjectsToSelect.ToArray(); };
            }
        }

        private void ConvertSingleToggle(Toggle toggle)
        {
            if (toggle == null)
                return;

            GameObject go = toggle.gameObject;

            Undo.SetCurrentGroupName("Convert to BetterToggle");
            int undoGroup = Undo.GetCurrentGroup();

            // ========== 保存属性 ==========
            bool interactable = toggle.interactable;
            Selectable.Transition transition = toggle.transition;
            ColorBlock colors = toggle.colors;
            SpriteState spriteState = toggle.spriteState;
            AnimationTriggers animationTriggers = CopyAnimationTriggers(toggle.animationTriggers);
            Graphic targetGraphic = toggle.targetGraphic;
            Navigation navigation = toggle.navigation;

            bool isOn = toggle.isOn;
            Toggle.ToggleTransition toggleTransition = toggle.toggleTransition;
            Graphic graphic = toggle.graphic;
            ToggleGroup group = toggle.group;

            // 保存事件
            SerializedObject serializedToggle = new SerializedObject(toggle);
            serializedToggle.Update();

            SerializedProperty onValueChangedProp = serializedToggle.FindProperty("onValueChanged");
            SerializedProperty persistentCalls = onValueChangedProp.FindPropertyRelative("m_PersistentCalls. m_Calls");

            int eventCount = persistentCalls != null ? persistentCalls.arraySize : 0;
            var eventData = new EventData[eventCount];

            for (int i = 0; i < eventCount; i++)
            {
                SerializedProperty call = persistentCalls.GetArrayElementAtIndex(i);
                eventData[i] = new EventData
                {
                    target = call.FindPropertyRelative("m_Target").objectReferenceValue,
                    methodName = call.FindPropertyRelative("m_MethodName").stringValue,
                    mode = call.FindPropertyRelative("m_Mode").intValue,
                    callState = call.FindPropertyRelative("m_CallState").intValue,
                    argFloat = call.FindPropertyRelative("m_Arguments.m_FloatArgument").floatValue,
                    argInt = call.FindPropertyRelative("m_Arguments. m_IntArgument").intValue,
                    argString = call.FindPropertyRelative("m_Arguments.m_StringArgument").stringValue,
                    argBool = call.FindPropertyRelative("m_Arguments.m_BoolArgument").boolValue,
                    argObject = call.FindPropertyRelative("m_Arguments.m_ObjectArgument").objectReferenceValue,
                    argObjectTypeName = call.FindPropertyRelative("m_Arguments.m_ObjectArgumentAssemblyTypeName")
                        .stringValue
                };
            }

            // ========== 删除并添加 ==========
            Undo.DestroyObjectImmediate(toggle);

            BetterToggle betterToggle = Undo.AddComponent<BetterToggle>(go);

            // ========== 恢复属性 ==========
            betterToggle.interactable = interactable;
            betterToggle.transition = transition;
            betterToggle.colors = colors;
            betterToggle.spriteState = spriteState;
            betterToggle.animationTriggers = animationTriggers;
            betterToggle.targetGraphic = targetGraphic;
            betterToggle.navigation = navigation;

            betterToggle.toggleTransition = toggleTransition;
            betterToggle.graphic = graphic;
            betterToggle.group = group;
            betterToggle.isOn = isOn;

            // ========== 恢复事件 ==========
            if (eventCount > 0)
            {
                SerializedObject newSerializedToggle = new SerializedObject(betterToggle);
                newSerializedToggle.Update();

                SerializedProperty newOnValueChangedProp = newSerializedToggle.FindProperty("onValueChanged");
                SerializedProperty newPersistentCalls =
                    newOnValueChangedProp.FindPropertyRelative("m_PersistentCalls.m_Calls");

                newPersistentCalls.ClearArray();

                for (int i = 0; i < eventCount; i++)
                {
                    newPersistentCalls.InsertArrayElementAtIndex(i);
                    SerializedProperty newCall = newPersistentCalls.GetArrayElementAtIndex(i);

                    newCall.FindPropertyRelative("m_Target").objectReferenceValue = eventData[i].target;
                    newCall.FindPropertyRelative("m_MethodName").stringValue = eventData[i].methodName;
                    newCall.FindPropertyRelative("m_Mode").intValue = eventData[i].mode;
                    newCall.FindPropertyRelative("m_CallState").intValue = eventData[i].callState;
                    newCall.FindPropertyRelative("m_Arguments.m_FloatArgument").floatValue = eventData[i].argFloat;
                    newCall.FindPropertyRelative("m_Arguments. m_IntArgument").intValue = eventData[i].argInt;
                    newCall.FindPropertyRelative("m_Arguments.m_StringArgument").stringValue = eventData[i].argString;
                    newCall.FindPropertyRelative("m_Arguments.m_BoolArgument").boolValue = eventData[i].argBool;
                    newCall.FindPropertyRelative("m_Arguments.m_ObjectArgument").objectReferenceValue =
                        eventData[i].argObject;
                    newCall.FindPropertyRelative("m_Arguments.m_ObjectArgumentAssemblyTypeName").stringValue =
                        eventData[i].argObjectTypeName;
                }

                newSerializedToggle.ApplyModifiedPropertiesWithoutUndo();
            }

            Undo.CollapseUndoOperations(undoGroup);
            EditorUtility.SetDirty(go);
        }

        private AnimationTriggers CopyAnimationTriggers(AnimationTriggers source)
        {
            if (source == null)
                return new AnimationTriggers();

            return new AnimationTriggers
            {
                normalTrigger = source.normalTrigger,
                highlightedTrigger = source.highlightedTrigger,
                pressedTrigger = source.pressedTrigger,
                selectedTrigger = source.selectedTrigger,
                disabledTrigger = source.disabledTrigger
            };
        }

        private struct EventData
        {
            public Object target;
            public string methodName;
            public int mode;
            public int callState;
            public float argFloat;
            public int argInt;
            public string argString;
            public bool argBool;
            public Object argObject;
            public string argObjectTypeName;
        }
    }
}