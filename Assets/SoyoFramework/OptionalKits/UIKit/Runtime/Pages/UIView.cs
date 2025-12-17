using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Pages
{
    public abstract class UIView : MonoBehaviour
    {
        #region Protected 子类可用

        protected IUIViewHost Host { get; private set; }

        protected abstract void OnInit();
        protected abstract void OnClose();

        #endregion

        #region 供 UIPage 调用

        internal void Init(IUIViewHost host)
        {
            Host = host;
            OnInit();
        }

        internal void Close()
        {
            OnClose();
        }

        #endregion
    }
}