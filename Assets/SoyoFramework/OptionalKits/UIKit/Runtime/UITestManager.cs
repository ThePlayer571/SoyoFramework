using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    internal class UITestManager : MonoBehaviour
    {
        public string UIPanelId;
        public UIRoot UIRoot;

        // todo
        // [Button("重新打开UIPanel")]
        private void OpenUIPanel()
        {
            if (string.IsNullOrEmpty(this.UIPanelId))
            {
                "UIPanelId不能为空".LogError();
                return;
            }

            UIKit.ClosePanel(UIPanelId);
            UIKit.OpenPanelAsync(UIPanelId).Forget();
        }

        private void Start()
        {
            UIRoot.gameObject.SetActive(false);
            OpenUIPanel();
        }
    }
}