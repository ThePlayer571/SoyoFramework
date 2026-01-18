using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Editor.TestSceneCreator
{
    /// <summary>
    /// 使用ScriptableSingleton存储UIKit测试场景创建器的配置
    /// </summary>
    [FilePath("ProjectSettings/UIKitTestSceneCreatorSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UIKitTestSceneCreatorSettings : ScriptableSingleton<UIKitTestSceneCreatorSettings>
    {
        [SerializeField] private UISettings _uiSettings;
        [SerializeField] private SceneAsset _sceneTemplate;
        [SerializeField] private string _outputFolderPath = "Assets/Scenes";

        public UISettings UISettings
        {
            get => _uiSettings;
            set => _uiSettings = value;
        }

        public SceneAsset SceneTemplate
        {
            get => _sceneTemplate;
            set => _sceneTemplate = value;
        }

        public string OutputFolderPath
        {
            get => _outputFolderPath;
            set => _outputFolderPath = value;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            Save(true);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _sceneTemplate = AssetDatabase.LoadAssetAtPath<SceneAsset>(Global.DefaultUITestScenePath);
        }
#endif
    }
}