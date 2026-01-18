using UnityEditor;
using UnityEngine;
using System.IO;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEditor.ProjectWindowCallback;

namespace SoyoFramework.OptionalKits.UIKit.Editor
{
    public static class CreateDefaultUIRoot
    {
        /// <summary>
        /// 在Create菜单中创建DefaultUIRoot
        /// </summary>
        [MenuItem("Assets/Create/SoyoFramework/UIKit/Default UIRoot", false, 1000)]
        public static void CreateUIRoot()
        {
            // 检查Prefab文件
            var uiRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Global.DefaultUIRootPath);
            if (uiRootPrefab == null)
            {
                Debug.LogError($"无法找到DefaultUIRoot，路径：{Global.DefaultUIRootPath}");
                return;
            }

            // 获取目标路径
            string targetDir = GetSelectedPathOrFallback();
            string newPrefabPath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(targetDir, "UIRoot.prefab"));

            // 复制Prefab
            AssetDatabase.CopyAsset(Global.DefaultUIRootPath, newPrefabPath);
            AssetDatabase.Refresh();

            // 自动进入重命名状态（类似SO创建）
            var newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
            ProjectWindowUtil.ShowCreatedAsset(newPrefab);
        }

        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    if (File.Exists(path))
                    {
                        path = Path.GetDirectoryName(path);
                    }
                    break;
                }
            }
            return path;
        }
    }
}