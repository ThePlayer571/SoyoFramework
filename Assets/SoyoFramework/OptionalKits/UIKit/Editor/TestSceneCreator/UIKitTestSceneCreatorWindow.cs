using System.IO;
using System.Linq;
using SoyoFramework.OptionalKits.UIKit.Editor.UITest;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoyoFramework.OptionalKits.UIKit.Editor.TestSceneCreator
{
    public class UIKitTestSceneCreatorWindow : EditorWindow
    {
        private UIKitTestSceneCreatorSettings _settings;
        private Vector2 _scrollPosition;
        private string[] _pageKeys;
        private int _selectedPageIndex = 0;
        private string _selectedPageKey; // 改为窗口成员

        [MenuItem("SoyoFramework/UIKit/测试场景创建器")]
        public static void ShowWindow()
        {
            var window = GetWindow<UIKitTestSceneCreatorWindow>("UIKit测试场景创建器");
            window.minSize = new Vector2(400, 350);
            window.Show();
        }

        private void OnEnable()
        {
            _settings = UIKitTestSceneCreatorSettings.instance;
            RefreshPageKeys();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("UIKit 测试场景创建器", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUI.BeginChangeCheck();

            // 1. UISettings 文件
            var previousUISettings = _settings.UISettings;
            _settings.UISettings = EditorGUILayout.ObjectField(
                "UISetting",
                _settings.UISettings,
                typeof(UISettings),
                false) as UISettings;

            // 检测UISettings是否变化
            bool uiSettingsChanged = previousUISettings != _settings.UISettings;
            
            if (EditorGUI.EndChangeCheck())
            {
                if (uiSettingsChanged)
                {
                    // UISettings变化时，重新刷新PageKeys
                    RefreshPageKeys();
                }
                _settings.Save();
            }

            // 定期检查UISettings内容是否被修改
            if (_settings.UISettings != null && Event.current.type == EventType.Layout)
            {
                CheckUISettingsModification();
            }

            EditorGUI.BeginChangeCheck();

            // 2. 场景模板文件
            _settings.SceneTemplate = EditorGUILayout.ObjectField(
                "场景模板",
                _settings.SceneTemplate,
                typeof(SceneAsset),
                false) as SceneAsset;

            // 3. 测试的Page的key (下拉框)
            EditorGUI.BeginDisabledGroup(_pageKeys == null || _pageKeys.Length == 0);
            
            if (_pageKeys != null && _pageKeys.Length > 0)
            {
                // 确保选中的索引有效
                if (!string.IsNullOrEmpty(_selectedPageKey))
                {
                    int foundIndex = System.Array.IndexOf(_pageKeys, _selectedPageKey);
                    if (foundIndex >= 0)
                    {
                        _selectedPageIndex = foundIndex;
                    }
                    else
                    {
                        // 如果之前选择的key不存在了，重置为第一个
                        _selectedPageIndex = 0;
                        _selectedPageKey = _pageKeys[0];
                    }
                }
                else if (_pageKeys.Length > 0)
                {
                    // 如果没有选择，默认选择第一个
                    _selectedPageIndex = 0;
                    _selectedPageKey = _pageKeys[0];
                }
                
                int newSelectedIndex = EditorGUILayout.Popup(
                    "测试的Page的key",
                    _selectedPageIndex,
                    _pageKeys);
                
                if (newSelectedIndex != _selectedPageIndex)
                {
                    _selectedPageIndex = newSelectedIndex;
                    _selectedPageKey = _pageKeys[_selectedPageIndex];
                }
            }
            else
            {
                EditorGUILayout.Popup("测试的Page的key", 0, new[] { "请先选择UISettings" });
            }
            
            EditorGUI.EndDisabledGroup();

            // 4. 输出路径（文件夹）
            EditorGUILayout.BeginHorizontal();
            _settings.OutputFolderPath = EditorGUILayout.TextField("输出路径", _settings.OutputFolderPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel(
                    "选择输出文件夹",
                    string.IsNullOrEmpty(_settings.OutputFolderPath) ? "Assets" : _settings.OutputFolderPath,
                    "");
                
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    // 转换为相对于项目的路径
                    if (selectedFolder.StartsWith(Application.dataPath))
                    {
                        selectedFolder = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
                    }
                    _settings.OutputFolderPath = selectedFolder;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                _settings.Save();
            }

            EditorGUILayout.Space(10);

            // 生成按钮
            bool hasError = HasGenerationError(out string errorMessage);
            EditorGUI.BeginDisabledGroup(hasError);
            if (GUILayout.Button("生成测试场景", GUILayout.Height(30)))
            {
                GenerateTestScene();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);

            // 在按钮下方显示提示信息
            if (hasError)
            {
                // 显示错误信息
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }
            else if (CanShowGenerationInfo())
            {
                // 显示将要生成的文件信息
                string fullOutputPath = GetFullOutputPath();
                EditorGUILayout.HelpBox(
                    $"将生成测试场景:\n{fullOutputPath}", 
                    MessageType.Info);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 检查UISettings内容是否被修改（PageConfigs变化）
        /// </summary>
        private void CheckUISettingsModification()
        {
            if (_settings.UISettings == null) return;

            var currentPageKeys = _settings.UISettings.PageConfigs
                ?.Where(config => !string.IsNullOrEmpty(config.PageName))
                .Select(config => config.PageName)
                .ToArray();

            // 比较当前的PageKeys和缓存的是否一致
            bool keysChanged = false;
            if (currentPageKeys == null && _pageKeys != null)
            {
                keysChanged = true;
            }
            else if (currentPageKeys != null && _pageKeys == null)
            {
                keysChanged = true;
            }
            else if (currentPageKeys != null && _pageKeys != null)
            {
                if (currentPageKeys.Length != _pageKeys.Length)
                {
                    keysChanged = true;
                }
                else
                {
                    for (int i = 0; i < currentPageKeys.Length; i++)
                    {
                        if (currentPageKeys[i] != _pageKeys[i])
                        {
                            keysChanged = true;
                            break;
                        }
                    }
                }
            }

            if (keysChanged)
            {
                RefreshPageKeys();
                Repaint(); // 强制重绘窗口
            }
        }

        private void RefreshPageKeys()
        {
            if (_settings.UISettings != null && _settings.UISettings.PageConfigs != null)
            {
                _pageKeys = _settings.UISettings.PageConfigs
                    .Where(config => !string.IsNullOrEmpty(config.PageName))
                    .Select(config => config.PageName)
                    .ToArray();

                // 如果当前选中的key不在新列表中，重置选择
                if (!string.IsNullOrEmpty(_selectedPageKey))
                {
                    if (_pageKeys.Contains(_selectedPageKey))
                    {
                        // 找到匹配的索引
                        _selectedPageIndex = System.Array.IndexOf(_pageKeys, _selectedPageKey);
                    }
                    else
                    {
                        // key不存在了，重置为第一个
                        _selectedPageIndex = 0;
                        _selectedPageKey = _pageKeys.Length > 0 ? _pageKeys[0] : null;
                    }
                }
                else if (_pageKeys.Length > 0)
                {
                    // 没有选择，默认选第一个
                    _selectedPageIndex = 0;
                    _selectedPageKey = _pageKeys[0];
                }
            }
            else
            {
                _pageKeys = null;
                _selectedPageIndex = 0;
                _selectedPageKey = null;
            }
        }

        private string GetGeneratedFileName()
        {
            if (string.IsNullOrEmpty(_selectedPageKey))
                return "";
            
            return $"{_selectedPageKey}TestScene.unity";
        }

        private string GetFullOutputPath()
        {
            if (string.IsNullOrEmpty(_settings.OutputFolderPath) || string.IsNullOrEmpty(_selectedPageKey))
                return "";
            
            return Path.Combine(_settings.OutputFolderPath, GetGeneratedFileName());
        }

        private bool CanShowGenerationInfo()
        {
            return _settings.UISettings != null &&
                   _settings.SceneTemplate != null &&
                   !string.IsNullOrEmpty(_selectedPageKey) &&
                   !string.IsNullOrEmpty(_settings.OutputFolderPath);
        }

        /// <summary>
        /// 检查是否有生成错误
        /// </summary>
        private bool HasGenerationError(out string errorMessage)
        {
            errorMessage = "";

            if (_settings.UISettings == null)
            {
                errorMessage = "错误:  请选择UISettings文件";
                return true;
            }

            if (_settings.SceneTemplate == null)
            {
                errorMessage = "错误: 请选择场景模板文件";
                return true;
            }

            if (string.IsNullOrEmpty(_selectedPageKey))
            {
                errorMessage = "错误: 请选择要测试的Page";
                return true;
            }

            if (string.IsNullOrEmpty(_settings.OutputFolderPath))
            {
                errorMessage = "错误: 请指定输出路径";
                return true;
            }

            if (!Directory.Exists(_settings.OutputFolderPath))
            {
                errorMessage = "错误: 输出路径不存在";
                return true;
            }

            // 检查目标文件是否已存在
            string fullOutputPath = GetFullOutputPath();
            if (File.Exists(fullOutputPath))
            {
                errorMessage = $"警告: 目标位置已存在同名文件\n{fullOutputPath}\n\n请删除现有文件或选择其他输出路径";
                return true;
            }

            return false;
        }

        private void GenerateTestScene()
        {
            try
            {
                // 获取场景模板路径
                string templatePath = AssetDatabase.GetAssetPath(_settings.SceneTemplate);
                
                // 生成完整的输出路径
                string fullOutputPath = GetFullOutputPath();
                
                // 保存当前场景
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }

                // 复制场景文件
                if (!AssetDatabase.CopyAsset(templatePath, fullOutputPath))
                {
                    EditorUtility.DisplayDialog("错误", "无法复制场景文件", "确定");
                    return;
                }

                AssetDatabase.Refresh();

                // 打开新场景
                Scene newScene = EditorSceneManager.OpenScene(fullOutputPath);

                // 查找UIPageTestManager
                UIPageTestManager testManager = FindObjectOfType<UIPageTestManager>();
                
                if (testManager == null)
                {
                    EditorUtility.DisplayDialog("错误", "在场景中未找到UIPageTestManager组件", "确定");
                    return;
                }

                // 配置UIPageTestManager
                testManager.ConfigureInternal(_selectedPageKey, _settings.UISettings);

                // 标记场景为已修改并保存
                EditorSceneManager.MarkSceneDirty(newScene);
                EditorSceneManager.SaveScene(newScene);

                // 显示成功提示
                EditorUtility.DisplayDialog(
                    "生成成功", 
                    $"测试场景已成功生成！\n\n文件路径:\n{fullOutputPath}\n\nPage Key: {_selectedPageKey}", 
                    "确定");
                
                Debug.Log($"<color=green>测试场景生成成功: </color> {fullOutputPath}");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"生成场景时发生错误:\n{e.Message}", "确定");
                Debug.LogError($"生成测试场景失败: {e}");
            }
        }
    }
}