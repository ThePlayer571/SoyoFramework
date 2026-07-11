using SoyoFramework.ToolKits;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Editor
{
    [CustomEditor(typeof(TriggerDetector))]
    public class TriggerDetectorEditor : UnityEditor.Editor
    {
        private SerializedProperty _debugAlwaysReturnHasTarget;
        private TriggerDetector _triggerDetector;

        private double _lastRepaintTime;
        private const double RepaintInterval = 0.25;

        private void OnEnable()
        {
            _triggerDetector = (TriggerDetector)target;
            _debugAlwaysReturnHasTarget = serializedObject.FindProperty("DebugAlwaysReturnHasTarget");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool isMultiEditing = targets.Length > 1;

            // 检查 Collider2D 配置
            DrawColliderValidation(isMultiEditing);

            EditorGUILayout.Space(10);

            // 设置
            DrawSettingsSection();

            EditorGUILayout.Space(10);

            // 调试选项
            DrawDebugSection();

            EditorGUILayout.Space(10);

            // 运行时信息
            if (isMultiEditing)
            {
                EditorGUILayout.LabelField("Runtime Information", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "同时编辑多个 TriggerDetector 时无法显示运行时信息。",
                    MessageType.Info);
            }
            else
            {
                DrawRuntimeInfo();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawColliderValidation(bool isMultiEditing)
        {
            // 多对象编辑时跳过collider验证
            if (isMultiEditing)
            {
                EditorGUILayout.HelpBox(
                    "Collider validation is disabled when editing multiple objects.",
                    MessageType.Info);
                return;
            }

            var collider2D = _triggerDetector.GetComponent<Collider2D>();

            if (collider2D == null)
            {
                EditorGUILayout.HelpBox(
                    "Missing Collider2D component!  TriggerDetector requires a Collider2D component to function.",
                    MessageType.Warning
                );

                if (GUILayout.Button("Add Collider2D"))
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    ShowColliderMenu(rect);
                }
            }
            else if (!collider2D.isTrigger)
            {
                EditorGUILayout.HelpBox(
                    $"Collider2D '{collider2D.GetType().Name}' is not set as Trigger!  Please enable 'Is Trigger' option.",
                    MessageType.Warning
                );

                if (GUILayout.Button("Set as Trigger"))
                {
                    Undo.RecordObject(collider2D, "Set Collider as Trigger");
                    collider2D.isTrigger = true;
                    EditorUtility.SetDirty(collider2D);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"✓ Collider2D ({collider2D.GetType().Name}) is properly configured as trigger.",
                    MessageType.Info
                );
            }
        }

        private void DrawSettingsSection()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                // RecordTarget 可编辑，通过 property setter 触发副作用
                EditorGUI.BeginChangeCheck();
                var newRecordTarget = EditorGUILayout.Toggle(
                    new GUIContent("Record Targets", "是否记录并存储具体的目标引用"),
                    _triggerDetector.RecordTarget);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Change RecordTarget");
                    _triggerDetector.RecordTarget = newRecordTarget;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        private void DrawDebugSection()
        {
            EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(_debugAlwaysReturnHasTarget,
                    new GUIContent("Always Return Has Target", "Force HasTarget to always return true for debugging"));
            }
        }

        private void DrawRuntimeInfo()
        {
            EditorGUILayout.LabelField("Runtime Information", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledGroupScope(true))
            using (new EditorGUI.IndentLevelScope())
            {
                // 基础信息 - 使用字段显示
                EditorGUILayout.Toggle("Has Target", _triggerDetector.HasTarget);
                EditorGUILayout.IntField("Target Count", _triggerDetector.TargetCount);
                EditorGUILayout.Toggle("Has Target Predicate", _triggerDetector.TargetPredicate != null);
                EditorGUILayout.Toggle("Has Comparer", _triggerDetector.Comparer != null);

                EditorGUILayout.Space(5);

                // 检测到的目标列表
                EditorGUILayout.LabelField("Detected Targets:", EditorStyles.boldLabel);

                if (_triggerDetector.RecordTarget && _triggerDetector.TargetCount > 0)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        int index = 0;
                        foreach (var target in _triggerDetector.DetectedTargets)
                        {
                            if (target != null)
                            {
                                EditorGUILayout.ObjectField(
                                    $"[{index}] {target.name}",
                                    target.gameObject, typeof(GameObject), true);
                            }
                            else
                            {
                                EditorGUILayout.TextField($"[{index}]", "NULL (Destroyed)");
                            }
                            index++;
                        }
                    }
                }
                else
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.TextField("Status", "None");
                    }
                }
            }

            // Play mode 时自动刷新（节流）
            if (Application.isPlaying)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("Runtime information updates automatically during Play Mode.", MessageType.Info);
                ThrottledRepaint();
            }
        }

        private void ThrottledRepaint()
        {
            var currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - _lastRepaintTime >= RepaintInterval)
            {
                _lastRepaintTime = currentTime;
                Repaint();
            }
        }

        private void ShowColliderMenu(Rect buttonRect)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Box Collider 2D"), false, () => AddCollider<BoxCollider2D>());
            menu.AddItem(new GUIContent("Circle Collider 2D"), false, () => AddCollider<CircleCollider2D>());
            menu.AddItem(new GUIContent("Capsule Collider 2D"), false, () => AddCollider<CapsuleCollider2D>());
            menu.AddItem(new GUIContent("Polygon Collider 2D"), false, () => AddCollider<PolygonCollider2D>());
            menu.DropDown(buttonRect);
        }

        private void AddCollider<T>() where T : Collider2D
        {
            Undo.RecordObject(_triggerDetector.gameObject, "Add Collider2D");
            var collider = _triggerDetector.gameObject.AddComponent<T>();
            collider.isTrigger = true;
            EditorUtility.SetDirty(_triggerDetector.gameObject);
        }
    }
}
