// 全是Claude写的，有点乱，来点好心人帮我重构喵
// 不得不说AI写Editor真的太好用了，这是我30分钟写出来的

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.ProcedureKit.Editor
{
    public class ProcedureKitEditorWindow : EditorWindow
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
        private const string ConfigPathPrefKey = "ProcedureKitEditor_ConfigPath";

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

        [MenuItem("SoyoFramework/Procedure Management")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProcedureKitEditorWindow>("Procedure Management");
            window.minSize = new Vector2(600, 400);
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
            EditorGUILayout.LabelField("ProcedureKitConfig", GUILayout.Width(120));
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

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

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
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        #region Validation

        private void ValidateAll()
        {
            _validationErrors.Clear();
            _duplicateProcedureNames.Clear();
            _duplicateTagNames.Clear();
            _duplicateProcedureEnumValues.Clear();
            _duplicateTagEnumValues.Clear();
            _invalidProcedureNames.Clear();
            _invalidTagNames.Clear();

            // 检查重复的 ProcedureId 名称
            var procedureNames = _procedures.Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
            _duplicateProcedureNames = procedureNames
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateProcedureNames)
            {
                _validationErrors.Add($"重复的 ProcedureId 名称: \"{dup}\"");
            }

            // 检查重复的 ProcedureId EnumValue
            var procedureEnumValues = _procedures.Select(p => p.EnumValue).ToList();
            _duplicateProcedureEnumValues = procedureEnumValues
                .GroupBy(v => v)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateProcedureEnumValues)
            {
                _validationErrors.Add($"重复的 ProcedureId 枚举值:  {dup}");
            }

            // 检查重复的 Tag 名称
            var tagNames = _tags.Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
            _duplicateTagNames = tagNames
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateTagNames)
            {
                _validationErrors.Add($"重复的 Tag 名称: \"{dup}\"");
            }

            // 检查重复的 Tag EnumValue
            var tagEnumValues = _tags.Select(t => t.EnumValue).ToList();
            _duplicateTagEnumValues = tagEnumValues
                .GroupBy(v => v)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateTagEnumValues)
            {
                _validationErrors.Add($"重复的 Tag 枚举值: {dup}");
            }

            // 检查 ProcedureId 命名规范
            foreach (var proc in _procedures)
            {
                if (!IsValidIdentifier(proc.Name))
                {
                    _invalidProcedureNames.Add(proc.Name);
                    _validationErrors.Add($"无效的 ProcedureId:  \"{proc.Name}\" - {GetIdentifierErrorReason(proc.Name)}");
                }
            }

            // 检查 Tag 命名规范
            foreach (var tag in _tags)
            {
                if (!IsValidIdentifier(tag.Name))
                {
                    _invalidTagNames.Add(tag.Name);
                    _validationErrors.Add($"无效的 Tag:  \"{tag.Name}\" - {GetIdentifierErrorReason(tag.Name)}");
                }
            }

            // 检查 Region 3 的配置
            ValidateRegion3Settings();
        }

        private void ValidateRegion3Settings()
        {
            if (!string.IsNullOrEmpty(_namespace))
            {
                var namespaceParts = _namespace.Split('.');
                foreach (var part in namespaceParts)
                {
                    if (!IsValidIdentifier(part))
                    {
                        _validationErrors.Add($"无效的命名空间部分: \"{part}\" - {GetIdentifierErrorReason(part)}");
                    }
                }
            }

            if (!IsValidIdentifier(_procedureIdEnumName))
            {
                _validationErrors.Add(
                    $"无效的 ProcedureId 枚举名:  \"{_procedureIdEnumName}\" - {GetIdentifierErrorReason(_procedureIdEnumName)}");
            }

            if (!IsValidIdentifier(_procedureTagEnumName))
            {
                _validationErrors.Add(
                    $"无效的 ProcedureTag 枚举名: \"{_procedureTagEnumName}\" - {GetIdentifierErrorReason(_procedureTagEnumName)}");
            }

            if (_procedureIdEnumName == _procedureTagEnumName)
            {
                _validationErrors.Add("ProcedureId 枚举名和 ProcedureTag 枚举名不能相同");
            }
        }

        private bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (!ValidIdentifierRegex.IsMatch(name))
                return false;

            if (CSharpKeywords.Contains(name))
                return false;

            return true;
        }

        private string GetIdentifierErrorReason(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "名称不能为空";

            if (char.IsDigit(name[0]))
                return "不能以数字开头";

            if (CSharpKeywords.Contains(name))
                return "不能使用C#关键字";

            if (!ValidIdentifierRegex.IsMatch(name))
                return "只能包含字母、数字和下划线，且必须以字母或下划线开头";

            return "未知错误";
        }

        private bool IsProcedureNameError(string name)
        {
            return _duplicateProcedureNames.Contains(name) || _invalidProcedureNames.Contains(name);
        }

        private bool IsProcedureEnumValueError(int enumValue)
        {
            return _duplicateProcedureEnumValues.Contains(enumValue);
        }

        private bool IsTagNameError(string name)
        {
            return _duplicateTagNames.Contains(name) || _invalidTagNames.Contains(name);
        }

        private bool IsTagEnumValueError(int enumValue)
        {
            return _duplicateTagEnumValues.Contains(enumValue);
        }

        #endregion

        #region Region 1:  ProcedureId & Tags

        private void DrawRegion1_ProcedureIdAndTags()
        {
            // 顶部按钮区域
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("按 Enum 值排序", GUILayout.Width(120)))
            {
                SortByEnumValue();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                int nextEnumValue = _tags.Count > 0 ? _tags.Max(t => t.EnumValue) + 1 : 0;
                _tags.Add(new TagEntry { Name = "NewTag", EnumValue = nextEnumValue });
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 表格头部 - Tag 的 EnumValue 行
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50); // EnumValue 列占位
            GUILayout.Space(120); // ProcedureId 列占位
            GUILayout.Space(22); // 删除按钮占位

            for (int i = 0; i < _tags.Count; i++)
            {
                bool isEnumValueError = IsTagEnumValueError(_tags[i].EnumValue);
                if (isEnumValueError)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    _tags[i].EnumValue = EditorGUILayout.IntField(_tags[i].EnumValue, GUILayout.Width(100));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    _tags[i].EnumValue = EditorGUILayout.IntField(_tags[i].EnumValue, GUILayout.Width(100));
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // 表格头部 - Tag 名称行
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("", GUILayout.Width(50)); // EnumValue 列占位
            EditorGUILayout.LabelField("ProcedureId", EditorStyles.boldLabel, GUILayout.Width(120));
            GUILayout.Space(22); // 删除按钮占位

            for (int i = 0; i < _tags.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(100));

                bool isErrorTag = IsTagNameError(_tags[i].Name);
                if (isErrorTag)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    _tags[i].Name = EditorGUILayout.TextField(_tags[i].Name, GUILayout.Width(75));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    _tags[i].Name = EditorGUILayout.TextField(_tags[i].Name, GUILayout.Width(75));
                }

                if (GUILayout.Button("×", GUILayout.Width(18)))
                {
                    RemoveTag(i);
                    i--;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // 表格内容
            for (int p = 0; p < _procedures.Count; p++)
            {
                var proc = _procedures[p];
                EditorGUILayout.BeginHorizontal();

                bool isEntrance = p == 0 && proc.Name == "Entrance";

                // EnumValue 输入框
                GUI.enabled = !isEntrance;
                bool isEnumValueError = IsProcedureEnumValueError(proc.EnumValue);
                if (isEnumValueError)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    proc.EnumValue = EditorGUILayout.IntField(proc.EnumValue, GUILayout.Width(50));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    proc.EnumValue = EditorGUILayout.IntField(proc.EnumValue, GUILayout.Width(50));
                }

                GUI.enabled = true;

                // ProcedureId 名称
                GUI.enabled = !isEntrance;
                bool isErrorProcedure = IsProcedureNameError(proc.Name);
                if (isErrorProcedure)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    proc.Name = EditorGUILayout.TextField(proc.Name, GUILayout.Width(120));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    proc.Name = EditorGUILayout.TextField(proc.Name, GUILayout.Width(120));
                }

                // 删除按钮
                if (!isEntrance && GUILayout.Button("×", GUILayout.Width(18)))
                {
                    RemoveProcedure(p);
                    p--;
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                else if (isEntrance)
                {
                    GUILayout.Space(22);
                }

                GUI.enabled = true;

                // Tags 勾选
                for (int t = 0; t < _tags.Count; t++)
                {
                    bool hasTag = proc.TagEnumValues.Contains(_tags[t].EnumValue);
                    var style = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 14,
                        alignment = TextAnchor.MiddleCenter
                    };

                    string buttonText = hasTag ? "✔" : "";
                    if (GUILayout.Button(buttonText, style, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (hasTag)
                            proc.TagEnumValues.Remove(_tags[t].EnumValue);
                        else
                            proc.TagEnumValues.Add(_tags[t].EnumValue);
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            // 添加Procedure按钮
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                int nextEnumValue = _procedures.Count > 0 ? _procedures.Max(p => p.EnumValue) + 1 : 0;
                _procedures.Add(new ProcedureEntry { Name = "NewProcedure", EnumValue = nextEnumValue });
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void SortByEnumValue()
        {
            // 保持 Entrance 在第一位，其余按 EnumValue 排序
            if (_procedures.Count > 1)
            {
                var entrance = _procedures[0];
                var others = _procedures.Skip(1).OrderBy(p => p.EnumValue).ToList();
                _procedures.Clear();
                _procedures.Add(entrance);
                _procedures.AddRange(others);
            }

            // Tag 按 EnumValue 排序
            _tags = _tags.OrderBy(t => t.EnumValue).ToList();
        }

        private void RemoveTag(int index)
        {
            int removedEnumValue = _tags[index].EnumValue;
            _tags.RemoveAt(index);

            // 从所有 procedure 中移除对该 Tag 的引用
            foreach (var proc in _procedures)
            {
                proc.TagEnumValues.Remove(removedEnumValue);
            }
        }

        private void RemoveProcedure(int index)
        {
            int removedEnumValue = _procedures[index].EnumValue;
            _procedures.RemoveAt(index);

            // 从所有 procedure 的 AllowedPreviousEnumValues 中移除对该 Procedure 的引用
            foreach (var proc in _procedures)
            {
                proc.AllowedPreviousEnumValues.Remove(removedEnumValue);
            }
        }

        #endregion

        #region Region 2: Allowed Previous Procedures

        private void DrawRegion2_AllowedPreviousProcedures()
        {
            // 跳过 Entrance (index 0)，从 index 1 开始遍历
            for (int procIndex = 1; procIndex < _procedures.Count; procIndex++)
            {
                var proc = _procedures[procIndex];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // 标题行
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{proc.EnumValue}] {proc.Name}", EditorStyles.boldLabel,
                    GUILayout.Width(180));
                EditorGUILayout.LabelField("← Allowed Previous:", GUILayout.Width(120));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                // 已选择的 AllowedPrevious
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);

                var toRemove = new List<int>();
                foreach (var prevEnumValue in proc.AllowedPreviousEnumValues)
                {
                    var prevProc = _procedures.FirstOrDefault(p => p.EnumValue == prevEnumValue);
                    if (prevProc != null)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(140));
                        EditorGUILayout.LabelField($"[{prevProc.EnumValue}] {prevProc.Name}", GUILayout.Width(110));
                        if (GUILayout.Button("×", GUILayout.Width(18)))
                        {
                            toRemove.Add(prevEnumValue);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                foreach (var enumValue in toRemove)
                    proc.AllowedPreviousEnumValues.Remove(enumValue);

                // 添加按钮
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    ShowAddPreviousProcedureMenu(proc);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            // 如果只有 Entrance，显示提示
            if (_procedures.Count <= 1)
            {
                EditorGUILayout.HelpBox("目前只有 Entrance，请先在 Region 1 添加其他 Procedure", MessageType.Info);
            }
        }

        private void ShowAddPreviousProcedureMenu(ProcedureEntry targetProc)
        {
            var menu = new GenericMenu();
            foreach (var proc in _procedures)
            {
                if (!targetProc.AllowedPreviousEnumValues.Contains(proc.EnumValue))
                {
                    int enumValue = proc.EnumValue;
                    menu.AddItem(new GUIContent($"[{proc.EnumValue}] {proc.Name}"), false,
                        () => { targetProc.AllowedPreviousEnumValues.Add(enumValue); });
                }
            }

            if (menu.GetItemCount() == 0)
                menu.AddDisabledItem(new GUIContent("(No more procedures to add)"));

            menu.ShowAsContext();
        }

        #endregion

        #region Region 3: Code Generation

        private void DrawRegion3_CodeGeneration()
        {
            EditorGUILayout.LabelField("代码生成设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // 生成路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成路径", GUILayout.Width(120));
            _generatePath = EditorGUILayout.TextField(_generatePath);
            if (GUILayout.Button("浏览", GUILayout.Width(50)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("选择生成路径", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        _generatePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("错误", "请选择项目 Assets 文件夹内的路径", "确定");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 命名空间
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("命名空间", GUILayout.Width(120));
            _namespace = EditorGUILayout.TextField(_namespace);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("枚举名称设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // ProcedureId 枚举名
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ProcedureId 枚举名", GUILayout.Width(120));
            _procedureIdEnumName = EditorGUILayout.TextField(_procedureIdEnumName);
            EditorGUILayout.EndHorizontal();

            // ProcedureTag 枚举名
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ProcedureTag 枚举名", GUILayout.Width(120));
            _procedureTagEnumName = EditorGUILayout.TextField(_procedureTagEnumName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            // 生成预览
            EditorGUILayout.LabelField("生成预览", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"将生成以下文件到:  {_generatePath}");
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"  • {_procedureIdEnumName}.cs", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"  • {_procedureTagEnumName}.cs", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"  • ProcedureConfig.cs", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"  • ProcedureManager.cs", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            // 生成按钮
            bool hasErrors = _validationErrors.Count > 0;
            GUI.enabled = !hasErrors;
            if (GUILayout.Button("生成 C# 类", GUILayout.Height(35)))
            {
                SaveToConfig();
                GenerateCSharpClasses();
            }

            GUI.enabled = true;

            if (hasErrors)
            {
                EditorGUILayout.HelpBox("存在验证错误，请先修复后再生成代码", MessageType.Warning);
            }
        }

        #endregion

        #region Save/Load

        private void LoadFromConfig()
        {
            // 加载 Tags
            _tags = new List<TagEntry>();
            foreach (var tag in _config.Tags)
            {
                _tags.Add(new TagEntry
                {
                    Name = tag.Name,
                    EnumValue = tag.EnumValue
                });
            }

            // 加载 Procedures
            _procedures = new List<ProcedureEntry>();
            foreach (var entry in _config.Procedures)
            {
                _procedures.Add(new ProcedureEntry
                {
                    Name = entry.Name,
                    EnumValue = entry.EnumValue,
                    TagEnumValues = new HashSet<int>(entry.TagEnumValues),
                    AllowedPreviousEnumValues = new List<int>(entry.AllowedPreviousEnumValues)
                });
            }

            // 确保 Entrance 存在
            if (_procedures.Count == 0 || _procedures[0].Name != "Entrance")
            {
                _procedures.Insert(0, new ProcedureEntry { Name = "Entrance", EnumValue = 0 });
            }

            // 确保 Entrance 的 EnumValue 为 0，且清除其 AllowedPreviousEnumValues
            _procedures[0].EnumValue = 0;
            _procedures[0].AllowedPreviousEnumValues.Clear();

            // 加载 Region 3 设置
            _generatePath = _config.GeneratePath;
            _namespace = _config.Namespace;
            _procedureIdEnumName = _config.ProcedureIdEnumName;
            _procedureTagEnumName = _config.ProcedureTagEnumName;
        }

        private void SaveToConfig()
        {
            // 保存 Tags
            _config.Tags = new List<ProcedureKitConfigSO.TagEntry>();
            foreach (var tag in _tags)
            {
                _config.Tags.Add(new ProcedureKitConfigSO.TagEntry
                {
                    Name = tag.Name,
                    EnumValue = tag.EnumValue
                });
            }

            // 保存 Procedures
            _config.Procedures = new List<ProcedureKitConfigSO.ProcedureEntry>();
            foreach (var proc in _procedures)
            {
                _config.Procedures.Add(new ProcedureKitConfigSO.ProcedureEntry
                {
                    Name = proc.Name,
                    EnumValue = proc.EnumValue,
                    TagEnumValues = proc.TagEnumValues.ToList(),
                    AllowedPreviousEnumValues = new List<int>(proc.AllowedPreviousEnumValues)
                });
            }

            // 保存 Region 3 设置
            _config.GeneratePath = _generatePath;
            _config.Namespace = _namespace;
            _config.ProcedureIdEnumName = _procedureIdEnumName;
            _config.ProcedureTagEnumName = _procedureTagEnumName;

            EditorUtility.SetDirty(_config);
            AssetDatabase.SaveAssets();
            Debug.Log("配置已保存!");
        }

        #endregion

        #region Code Generation

        private void GenerateCSharpClasses()
        {
            if (!Directory.Exists(_generatePath))
                Directory.CreateDirectory(_generatePath);

            GenerateProcedureIdFile();
            GenerateProcedureTagFile();
            GenerateProcedureConfigFile();
            GenerateProcedureManagerFile();

            // 保存当前编辑数据的副本
            var savedTags = _tags.Select(t => new TagEntry { Name = t.Name, EnumValue = t.EnumValue }).ToList();
            var savedProcedures = _procedures.Select(p => new ProcedureEntry
            {
                Name = p.Name,
                EnumValue = p.EnumValue,
                TagEnumValues = new HashSet<int>(p.TagEnumValues),
                AllowedPreviousEnumValues = new List<int>(p.AllowedPreviousEnumValues)
            }).ToList();

            AssetDatabase.Refresh();

            // 恢复编辑数据
            _tags = savedTags;
            _procedures = savedProcedures;

            Debug.Log($"C# 类已生成到: {_generatePath}");
        }

        private void GenerateProcedureIdFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by ProcedureKit Editor");
            sb.AppendLine("// Do not modify this file manually");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {_procedureIdEnumName}");
            sb.AppendLine("    {");

            // 按 EnumValue 排序输出
            var sortedProcedures = _procedures.OrderBy(p => p.EnumValue).ToList();
            for (int i = 0; i < sortedProcedures.Count; i++)
            {
                var proc = sortedProcedures[i];
                string comma = i < sortedProcedures.Count - 1 ? "," : "";
                sb.AppendLine($"        {proc.Name} = {proc.EnumValue}{comma}");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            string filePath = Path.Combine(_generatePath, $"{_procedureIdEnumName}.cs");
            File.WriteAllText(filePath, sb.ToString());
        }

        private void GenerateProcedureTagFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by ProcedureKit Editor");
            sb.AppendLine("// Do not modify this file manually");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {_procedureTagEnumName}");
            sb.AppendLine("    {");

            // 按 EnumValue 排序输出
            var sortedTags = _tags.OrderBy(t => t.EnumValue).ToList();
            for (int i = 0; i < sortedTags.Count; i++)
            {
                var tag = sortedTags[i];
                string comma = i < sortedTags.Count - 1 ? "," : "";
                sb.AppendLine($"        {tag.Name} = {tag.EnumValue}{comma}");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            string filePath = Path.Combine(_generatePath, $"{_procedureTagEnumName}.cs");
            File.WriteAllText(filePath, sb.ToString());
        }

        private void GenerateProcedureConfigFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by ProcedureKit Editor");
            sb.AppendLine("// Do not modify this file manually");
            sb.AppendLine();
            sb.AppendLine("using System. Collections.Generic;");
            sb.AppendLine("using System.Diagnostics. CodeAnalysis;");
            sb.AppendLine("using SoyoFramework.OptionalKits. ProcedureKit. Runtime. DataClasses;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
            sb.AppendLine(
                $"    public class ProcedureConfig :  ProcedureConfig<{_procedureIdEnumName}, {_procedureTagEnumName}>");
            sb.AppendLine("    {");
            sb.AppendLine(
                $"        public override {_procedureIdEnumName} InitialProcedure {{ get; }} = {_procedureIdEnumName}. Entrance;");
            sb.AppendLine();
            sb.AppendLine("        [NotNull]");
            sb.AppendLine(
                $"        public override Dictionary<{_procedureIdEnumName}, MetaData> MetaDatas {{ get; }} = new()");
            sb.AppendLine("        {");

            // 按 EnumValue 排序输出
            var sortedProcedures = _procedures.OrderBy(p => p.EnumValue).ToList();
            for (int i = 0; i < sortedProcedures.Count; i++)
            {
                var proc = sortedProcedures[i];

                // 生成 Tags 数组
                string tagsArray;
                if (proc.TagEnumValues.Count > 0)
                {
                    var tagNames = proc.TagEnumValues
                        .OrderBy(ev => ev)
                        .Select(ev => _tags.FirstOrDefault(t => t.EnumValue == ev))
                        .Where(t => t != null)
                        .Select(t => $"{_procedureTagEnumName}.{t.Name}");
                    tagsArray = $"new {_procedureTagEnumName}[] {{ {string.Join(", ", tagNames)} }}";
                }
                else
                {
                    tagsArray = $"new {_procedureTagEnumName}[] {{ }}";
                }

                // 生成 AllowedPreviousProcedures 数组
                string allowedPrevArray;
                if (proc.AllowedPreviousEnumValues.Count > 0)
                {
                    var prevNames = proc.AllowedPreviousEnumValues
                        .Select(ev => _procedures.FirstOrDefault(p => p.EnumValue == ev))
                        .Where(p => p != null)
                        .Select(p => $"{_procedureIdEnumName}. {p.Name}");
                    allowedPrevArray = $"new {_procedureIdEnumName}[] {{ {string.Join(", ", prevNames)} }}";
                }
                else
                {
                    allowedPrevArray = $"new {_procedureIdEnumName}[] {{ }}";
                }

                string comma = i < sortedProcedures.Count - 1 ? "," : "";

                sb.AppendLine("            {");
                sb.AppendLine($"                {_procedureIdEnumName}. {proc.Name},");
                sb.AppendLine("                new MetaData(");
                sb.AppendLine($"                    {tagsArray},");
                sb.AppendLine($"                    {allowedPrevArray}");
                sb.AppendLine("                )");
                sb.AppendLine($"            }}{comma}");
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            string filePath = Path.Combine(_generatePath, "ProcedureConfig.cs");
            File.WriteAllText(filePath, sb.ToString());
        }

        private void GenerateProcedureManagerFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by ProcedureKit Editor");
            sb.AppendLine("// Do not modify this file manually");
            sb.AppendLine();
            sb.AppendLine("using SoyoFramework. OptionalKits. ProcedureKit.Runtime.Core;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
            sb.AppendLine(
                $"    public interface IProcedureManager :  IProcedureManager<{_procedureIdEnumName}, {_procedureTagEnumName}>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine(
                $"    public class ProcedureManager : ProcedureManager<{_procedureIdEnumName}, {_procedureTagEnumName}>, IProcedureManager");
            sb.AppendLine("    {");
            sb.AppendLine("        private ProcedureManager(ProcedureConfig config) : base(config)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static IProcedureManager CreateInstance()");
            sb.AppendLine("        {");
            sb.AppendLine("            var config = new ProcedureConfig();");
            sb.AppendLine("            return new ProcedureManager(config);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            string filePath = Path.Combine(_generatePath, "ProcedureManager.cs");
            File.WriteAllText(filePath, sb.ToString());
        }

        #endregion
    }
}