using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    public abstract class UIPanelBase : MonoBehaviour, IUIPanel
    {
    }

    public abstract class UIMainPanelBase : UIPanelBase, IUIMainPanel
    {
        protected virtual void OnInit(object initData)
        {
        }

        protected virtual void OnClose()
        {
        }

        void IUIMainPanel.Init(object initData)
        {
            OnInit(initData);
        }

        void IUIMainPanel.Close()
        {
            OnClose();
            gameObject.DestroySelf();
        }
    }
}