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

            if (GUILayout.Button("移除空值 & 扫描所有View并添加", GUILayout.Height(40)))
            {
                var viewList = page._views;
                // 移除空值
                for (int i = viewList.Count - 1; i >= 0; i--)
                {
                    if (viewList[i] == null)
                    {
                        viewList.RemoveAt(i);
                    }
                }
                
                // 扫描所有子对象的UIView
                var views = page.GetComponentsInChildren<UIView>(true);
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