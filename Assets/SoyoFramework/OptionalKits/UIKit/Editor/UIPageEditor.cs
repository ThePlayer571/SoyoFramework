using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Editor
{
    [CustomEditor(typeof(UIPage), true)]
    public class UIPageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIPage page = (UIPage)target;

            if (GUILayout.Button("扫描所有View并添加", GUILayout.Height(40)))
            {
                // 扫描所有子对象的UIView
                var views = page.GetComponentsInChildren<UIView>(true);
                var viewList = page._views;
                int added = 0;
                foreach (var view in views)
                {
                    if (!viewList.Contains(view))
                    {
                        viewList.Add(view);
                        added++;
                    }
                }
                if (added > 0)
                {
                    EditorUtility.SetDirty(page);
                }
            }
        }
    }
}