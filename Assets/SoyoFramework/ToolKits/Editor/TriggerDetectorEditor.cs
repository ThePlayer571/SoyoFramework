using SoyoFramework.ToolKits.Runtime;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.ToolKits.Editor
{
    [CustomEditor(typeof(TriggerDetector))]
    public class TriggerDetectorEditor : UnityEditor.Editor
    {
        private SerializedProperty _debugAlwaysReturnHasTarget;
        private TriggerDetector _triggerDetector;

        private void OnEnable()
        {
            _triggerDetector = (TriggerDetector)target;
            _debugAlwaysReturnHasTarget = serializedObject.FindProperty("DebugAlwaysReturnHasTarget");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 检查 Collider2D 配置
            DrawColliderValidation();

            EditorGUILayout.Space(10);

            // 调试选项
            DrawDebugSection();

            EditorGUILayout.Space(10);

            // 运行时信息
            DrawRuntimeInfo();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawColliderValidation()
        {
            var collider2D = _triggerDetector.GetComponent<Collider2D>();

            if (collider2D == null)
            {
                EditorGUILayout.HelpBox(
                    "Missing Collider2D component!  TriggerDetector requires a Collider2D component to function.",
                    MessageType.Warning
                );

                if (GUILayout.Button("Add Collider2D"))
                {
                    ShowColliderMenu();
                }
            }
            else if (! collider2D.isTrigger)
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

        private void DrawDebugSection()
        {
            EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout. PropertyField(_debugAlwaysReturnHasTarget, 
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
                EditorGUILayout.IntField("Target Count", _triggerDetector. TargetCount);
                EditorGUILayout.Toggle("Record Targets", _triggerDetector.RecordTarget);
                EditorGUILayout.Toggle("Has Target Predicate", _triggerDetector. TargetPredicate != null);
                EditorGUILayout.Toggle("Has Comparer", _triggerDetector. Comparer != null);

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
                                EditorGUILayout.ObjectField($"[{index}]", target.gameObject, typeof(GameObject), true);
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

            // Play mode 时自动刷新
            if (Application.isPlaying)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout. HelpBox("Runtime information updates automatically during Play Mode.", MessageType.Info);
                Repaint();
            }
        }

        private void ShowColliderMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Box Collider 2D"), false, () => AddCollider<BoxCollider2D>());
            menu.AddItem(new GUIContent("Circle Collider 2D"), false, () => AddCollider<CircleCollider2D>());
            menu.AddItem(new GUIContent("Capsule Collider 2D"), false, () => AddCollider<CapsuleCollider2D>());
            menu.AddItem(new GUIContent("Polygon Collider 2D"), false, () => AddCollider<PolygonCollider2D>());
            menu.ShowAsContext();
        }

        private void AddCollider<T>() where T : Collider2D
        {
            Undo.RecordObject(_triggerDetector. gameObject, "Add Collider2D");
            var collider = _triggerDetector.gameObject.AddComponent<T>();
            collider.isTrigger = true;
            EditorUtility. SetDirty(_triggerDetector.gameObject);
        }
    }
}