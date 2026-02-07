// 全是Claude写的
// 不得不说AI写Editor真的太好用了，这是我30分钟写出来的

using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.ProcedureKit.Editor.Window
{
    public partial class ProcedureKitEditorWindow : EditorWindow
    {
        private ProcedureKitConfigSO _config;
        private Vector2 _scrollPosition;
        private int _selectedRegion = 0;

        private readonly string[] _regionNames =
            { "ProcedureId & Tags", "Allowed Previous Procedures", "Code Generation" };

        // 临时编辑数据 - Region 1 & 2
        private List<TagEntry> _tags = new List<TagEntry>();
        private List<ProcedureEntry> _procedures = new List<ProcedureEntry>();

        // 临时编辑数据 - Region 3
        private string _generatePath = "Assets/Scripts/ProcedureKit/Generated";
        private string _namespace = "Game. Procedures";
        private string _procedureIdEnumName = "ProcedureId";
        private string _procedureTagEnumName = "ProcedureTag";

        // 验证错误缓存
        private List<string> _validationErrors = new List<string>();
        private HashSet<string> _duplicateProcedureNames = new HashSet<string>();
        private HashSet<string> _duplicateTagNames = new HashSet<string>();
        private HashSet<int> _duplicateProcedureEnumValues = new HashSet<int>();
        private HashSet<int> _duplicateTagEnumValues = new HashSet<int>();
        private HashSet<string> _invalidProcedureNames = new HashSet<string>();
        private HashSet<string> _invalidTagNames = new HashSet<string>();

        // EditorPrefs 键名
        public const string ConfigPathPrefKey = "ProcedureKitEditor_ConfigPath";

        // C# 标识符验证正则：以字母或下划线开头，后面可以是字母、数字或下划线
        private static readonly Regex ValidIdentifierRegex =
            new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        // C# 关键字列表
        private static readonly HashSet<string> CSharpKeywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
            "void", "volatile", "while"
        };

        private class TagEntry
        {
            public string Name;
            public int EnumValue;
        }

        private class ProcedureEntry
        {
            public string Name;
            public int EnumValue;
            public HashSet<int> TagEnumValues = new HashSet<int>(); // 存储选中的Tag的EnumValue
            public List<int> AllowedPreviousEnumValues = new List<int>(); // 存储AllowedPrevious的EnumValue
        }

        [MenuItem("SoyoFramework/Procedure Management")]
        public static void ShowWindow()
        {
            var window = GetWindow<Window.ProcedureKitEditorWindow>("Procedure Management");
            window. minSize = new Vector2(600, 400);
        }

        private void OnEnable()
        {
            LoadRememberedConfig();
        }

        private void LoadRememberedConfig()
        {
            string savedPath = EditorPrefs.GetString(ConfigPathPrefKey, string.Empty);
            if (!string.IsNullOrEmpty(savedPath))
            {
                var config = AssetDatabase.LoadAssetAtPath<ProcedureKitConfigSO>(savedPath);
                if (config != null)
                {
                    _config = config;
                    LoadFromConfig();
                }
                else
                {
                    EditorPrefs.DeleteKey(ConfigPathPrefKey);
                }
            }
        }

        private void SaveConfigPath()
        {
            if (_config != null)
            {
                string path = AssetDatabase.GetAssetPath(_config);
                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString(ConfigPathPrefKey, path);
                }
            }
            else
            {
                EditorPrefs.DeleteKey(ConfigPathPrefKey);
            }
        }

        private void OnGUI()
        {
            ValidateAll();

            bool hasErrors = _validationErrors.Count > 0;

            EditorGUILayout.Space(10);

            // 顶部配置区域
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout. LabelField("ProcedureKitConfig", GUILayout.Width(120));
            var newConfig =
                (ProcedureKitConfigSO)EditorGUILayout.ObjectField(_config, typeof(ProcedureKitConfigSO), false);
            EditorGUILayout.EndHorizontal();

            if (newConfig != _config)
            {
                _config = newConfig;
                SaveConfigPath();
                if (_config != null)
                    LoadFromConfig();
            }

            if (_config == null)
            {
                EditorGUILayout.HelpBox("请拖入 ProcedureKitConfig SO 文件", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(5);

            // 保存按钮
            GUI.enabled = !hasErrors;
            if (GUILayout.Button("保存配置信息", GUILayout.Height(30)))
            {
                SaveToConfig();
            }

            GUI.enabled = true;

            EditorGUILayout.Space(10);

            // Region 选择
            _selectedRegion = GUILayout.Toolbar(_selectedRegion, _regionNames);

            EditorGUILayout.Space(10);

            // 主内容区域
            EditorGUILayout.BeginVertical();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout. ExpandHeight(true));

            switch (_selectedRegion)
            {
                case 0:
                    DrawRegion1_ProcedureIdAndTags();
                    break;
                case 1:
                    DrawRegion2_AllowedPreviousProcedures();
                    break;
                case 2:
                    DrawRegion3_CodeGeneration();
                    break;
            }

            EditorGUILayout.EndScrollView();

            // 底部错误信息区域
            if (hasErrors)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("验证错误", EditorStyles.boldLabel);
                foreach (var error in _validationErrors)
                {
                    EditorGUILayout.HelpBox(error, MessageType. Error);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
    }
}