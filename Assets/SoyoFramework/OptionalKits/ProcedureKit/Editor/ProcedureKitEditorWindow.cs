// 这个文件全是 Claude 写的，太牛逼了

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
        private List<string> _tagNames = new List<string>();
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
        private HashSet<string> _invalidProcedureNames = new HashSet<string>();
        private HashSet<string> _invalidTagNames = new HashSet<string>();

        // EditorPrefs 键名
        private const string ConfigPathPrefKey = "ProcedureKitEditor_ConfigPath";

        private class ProcedureEntry
        {
            public string Name;
            public HashSet<int> TagIndices = new HashSet<int>();
            public List<int> AllowedPreviousIndices = new List<int>();
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
            // 窗口启用时，尝试加载上次使用的配置文件
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
                    // 文件不存在了，清除记忆
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
            // 每帧更新验证状态
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
                SaveConfigPath(); // 保存配置文件路径
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

            // 主内容区域 (使用 FlexibleSpace 让错误框固定在底部)
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

            // 底部错误信息区域 (固定在底部)
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
            _invalidProcedureNames.Clear();
            _invalidTagNames.Clear();

            // 检查重复的 ProcedureId
            var procedureNames = _procedures.Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
            _duplicateProcedureNames = procedureNames
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateProcedureNames)
            {
                _validationErrors.Add($"重复的 ProcedureId: \"{dup}\"");
            }

            // 检查重复的 Tag
            var tagNames = _tagNames.Where(n => !string.IsNullOrEmpty(n)).ToList();
            _duplicateTagNames = tagNames
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateTagNames)
            {
                _validationErrors.Add($"重复的 Tag: \"{dup}\"");
            }

            // 检查 ProcedureId 命名规范
            foreach (var proc in _procedures)
            {
                if (!IsValidIdentifier(proc.Name))
                {
                    _invalidProcedureNames.Add(proc.Name);
                    _validationErrors.Add($"无效的 ProcedureId: \"{proc.Name}\" - {GetIdentifierErrorReason(proc.Name)}");
                }
            }

            // 检查 Tag 命名规范
            foreach (var tag in _tagNames)
            {
                if (!IsValidIdentifier(tag))
                {
                    _invalidTagNames.Add(tag);
                    _validationErrors.Add($"无效的 Tag: \"{tag}\" - {GetIdentifierErrorReason(tag)}");
                }
            }

            // 检查 Region 3 的配置
            ValidateRegion3Settings();
        }

        private void ValidateRegion3Settings()
        {
            // 验证命名空间
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

            // 验证枚举名称
            if (!IsValidIdentifier(_procedureIdEnumName))
            {
                _validationErrors.Add(
                    $"无效的 ProcedureId 枚举名:  \"{_procedureIdEnumName}\" - {GetIdentifierErrorReason(_procedureIdEnumName)}");
            }

            if (!IsValidIdentifier(_procedureTagEnumName))
            {
                _validationErrors.Add(
                    $"无效的 ProcedureTag 枚举名:  \"{_procedureTagEnumName}\" - {GetIdentifierErrorReason(_procedureTagEnumName)}");
            }

            // 检查枚举名称是否重复
            if (_procedureIdEnumName == _procedureTagEnumName)
            {
                _validationErrors.Add("ProcedureId 枚举名和 ProcedureTag 枚举名不能相同");
            }
        }

        private bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            // 检查是否符合C#标识符规则
            if (!ValidIdentifierRegex.IsMatch(name))
                return false;

            // 检查是否是C#关键字
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

        private bool IsTagNameError(string name)
        {
            return _duplicateTagNames.Contains(name) || _invalidTagNames.Contains(name);
        }

        #endregion

        #region Region 1: ProcedureId & Tags

        private void DrawRegion1_ProcedureIdAndTags()
        {
            // 添加Tag按钮 (右上角)
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                _tagNames.Add("NewTag");
            }

            EditorGUILayout.EndHorizontal();

            // 表格头部
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("ProcedureId", EditorStyles.boldLabel, GUILayout.Width(150));
            for (int i = 0; i < _tagNames.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(100));
                EditorGUILayout.BeginHorizontal();

                // 高亮错误的 Tag
                bool isErrorTag = IsTagNameError(_tagNames[i]);
                if (isErrorTag)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    _tagNames[i] = EditorGUILayout.TextField(_tagNames[i], GUILayout.Width(80));
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    _tagNames[i] = EditorGUILayout.TextField(_tagNames[i], GUILayout.Width(80));
                }

                if (GUILayout.Button("×", GUILayout.Width(18)))
                {
                    RemoveTag(i);
                    i--;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // 表格内容
            for (int p = 0; p < _procedures.Count; p++)
            {
                var proc = _procedures[p];
                EditorGUILayout.BeginHorizontal();

                // ProcedureId 名称
                bool isEntrance = p == 0 && proc.Name == "Entrance";
                GUI.enabled = !isEntrance;

                EditorGUILayout.BeginHorizontal(GUILayout.Width(150));

                // 高亮错误的 ProcedureId
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

                if (!isEntrance && GUILayout.Button("×", GUILayout.Width(18)))
                {
                    RemoveProcedure(p);
                    p--;
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                EditorGUILayout.EndHorizontal();

                GUI.enabled = true;

                // Tags 勾选
                for (int t = 0; t < _tagNames.Count; t++)
                {
                    bool hasTag = proc.TagIndices.Contains(t);
                    var style = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 14,
                        alignment = TextAnchor.MiddleCenter
                    };

                    string buttonText = hasTag ? "✔" : "";
                    if (GUILayout.Button(buttonText, style, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (hasTag)
                            proc.TagIndices.Remove(t);
                        else
                            proc.TagIndices.Add(t);
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            // 添加Procedure按钮 (左下角)
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                _procedures.Add(new ProcedureEntry { Name = "NewProcedure" });
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void RemoveTag(int index)
        {
            _tagNames.RemoveAt(index);
            // 更新所有procedure的TagIndices
            foreach (var proc in _procedures)
            {
                proc.TagIndices.Remove(index);
                var newIndices = new HashSet<int>();
                foreach (var i in proc.TagIndices)
                {
                    if (i > index)
                        newIndices.Add(i - 1);
                    else
                        newIndices.Add(i);
                }

                proc.TagIndices = newIndices;
            }
        }

        private void RemoveProcedure(int index)
        {
            _procedures.RemoveAt(index);
            // 更新所有procedure的AllowedPreviousIndices
            foreach (var proc in _procedures)
            {
                proc.AllowedPreviousIndices.Remove(index);
                for (int i = 0; i < proc.AllowedPreviousIndices.Count; i++)
                {
                    if (proc.AllowedPreviousIndices[i] > index)
                        proc.AllowedPreviousIndices[i]--;
                }
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
                EditorGUILayout.LabelField(proc.Name, EditorStyles.boldLabel, GUILayout.Width(150));
                EditorGUILayout.LabelField("← Allowed Previous:", GUILayout.Width(120));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                // 已选择的 AllowedPrevious
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);

                var toRemove = new List<int>();
                foreach (var prevIndex in proc.AllowedPreviousIndices)
                {
                    if (prevIndex >= 0 && prevIndex < _procedures.Count)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(120));
                        EditorGUILayout.LabelField(_procedures[prevIndex].Name, GUILayout.Width(90));
                        if (GUILayout.Button("×", GUILayout.Width(18)))
                        {
                            toRemove.Add(prevIndex);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                foreach (var idx in toRemove)
                    proc.AllowedPreviousIndices.Remove(idx);

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
            for (int i = 0; i < _procedures.Count; i++)
            {
                if (!targetProc.AllowedPreviousIndices.Contains(i))
                {
                    int index = i;
                    menu.AddItem(new GUIContent(_procedures[i].Name), false,
                        () => { targetProc.AllowedPreviousIndices.Add(index); });
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
                    // 转换为相对路径
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
            _tagNames = new List<string>(_config.TagNames);
            _procedures = new List<ProcedureEntry>();

            foreach (var entry in _config.Procedures)
            {
                _procedures.Add(new ProcedureEntry
                {
                    Name = entry.Name,
                    TagIndices = new HashSet<int>(entry.TagIndices),
                    AllowedPreviousIndices = new List<int>(entry.AllowedPreviousIndices)
                });
            }

            // 确保Entrance存在
            if (_procedures.Count == 0 || _procedures[0].Name != "Entrance")
            {
                _procedures.Insert(0, new ProcedureEntry { Name = "Entrance" });
            }

            // 清除 Entrance 的 AllowedPreviousIndices（如果有的话）
            if (_procedures.Count > 0)
            {
                _procedures[0].AllowedPreviousIndices.Clear();
            }

            // 加载 Region 3 设置
            _generatePath = _config.GeneratePath;
            _namespace = _config.Namespace;
            _procedureIdEnumName = _config.ProcedureIdEnumName;
            _procedureTagEnumName = _config.ProcedureTagEnumName;
        }

        private void SaveToConfig()
        {
            _config.TagNames = new List<string>(_tagNames);
            _config.Procedures = new List<ProcedureKitConfigSO.ProcedureEntry>();

            foreach (var proc in _procedures)
            {
                _config.Procedures.Add(new ProcedureKitConfigSO.ProcedureEntry
                {
                    Name = proc.Name,
                    TagIndices = proc.TagIndices.ToList(),
                    AllowedPreviousIndices = new List<int>(proc.AllowedPreviousIndices)
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
            // 确保目录存在
            if (!Directory.Exists(_generatePath))
                Directory.CreateDirectory(_generatePath);

            // 生成 ProcedureId 枚举
            GenerateProcedureIdFile();

            // 生成 ProcedureTag 枚举
            GenerateProcedureTagFile();

            // 生成 ProcedureConfig 类
            GenerateProcedureConfigFile();

            // 生成 ProcedureManager 类
            GenerateProcedureManagerFile();

            // 保存当前编辑数据的副本
            var savedTagNames = new List<string>(_tagNames);
            var savedProcedures = _procedures.Select(p => new ProcedureEntry
            {
                Name = p.Name,
                TagIndices = new HashSet<int>(p.TagIndices),
                AllowedPreviousIndices = new List<int>(p.AllowedPreviousIndices)
            }).ToList();

            AssetDatabase.Refresh();

            // 恢复编辑数据
            _tagNames = savedTagNames;
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

            for (int i = 0; i < _procedures.Count; i++)
            {
                var proc = _procedures[i];
                string comma = i < _procedures.Count - 1 ? "," : "";
                sb.AppendLine($"        {proc.Name} = {i}{comma}");
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

            for (int i = 0; i < _tagNames.Count; i++)
            {
                string comma = i < _tagNames.Count - 1 ? "," : "";
                sb.AppendLine($"        {_tagNames[i]} = {i}{comma}");
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
            sb.AppendLine("using System.Collections.Generic;");
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

            for (int i = 0; i < _procedures.Count; i++)
            {
                var proc = _procedures[i];

                // 生成 Tags 数组
                string tagsArray;
                if (proc.TagIndices.Count > 0)
                {
                    var tagNames = proc.TagIndices
                        .OrderBy(x => x)
                        .Where(x => x < _tagNames.Count)
                        .Select(x => $"{_procedureTagEnumName}.{_tagNames[x]}");
                    tagsArray = $"new {_procedureTagEnumName}[] {{ {string.Join(", ", tagNames)} }}";
                }
                else
                {
                    tagsArray = $"new {_procedureTagEnumName}[] {{ }}";
                }

                // 生成 AllowedPreviousProcedures 数组
                string allowedPrevArray;
                if (proc.AllowedPreviousIndices.Count > 0)
                {
                    var prevNames = proc.AllowedPreviousIndices
                        .Where(x => x < _procedures.Count)
                        .Select(x => $"{_procedureIdEnumName}.{_procedures[x].Name}");
                    allowedPrevArray = $"new {_procedureIdEnumName}[] {{ {string.Join(", ", prevNames)} }}";
                }
                else
                {
                    allowedPrevArray = $"new {_procedureIdEnumName}[] {{ }}";
                }

                string comma = i < _procedures.Count - 1 ? "," : "";

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