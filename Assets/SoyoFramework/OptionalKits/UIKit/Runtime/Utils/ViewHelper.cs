using System;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils
{
    [Experimental]
    public class ViewHelper
    {
        private Action _onShow;
        private Action _onHide;
        private Action _onShowInstantly;
        private Action _onHideInstantly;

        public void Show()
        {
            _onShow?.Invoke();
        }

        public void Hide()
        {
            _onHide?.Invoke();
        }

        public void ShowInstantly()
        {
            _onShowInstantly?.Invoke();
        }

        public void HideInstantly()
        {
            _onHideInstantly?.Invoke();
        }

        public ViewHelper WithOnShow(Action onShow)
        {
            _onShow = onShow;
            return this;
        }

        public ViewHelper WithOnHide(Action onHide)
        {
            _onHide = onHide;
            return this;
        }

        public ViewHelper WithOnShowInstantly(Action onShowInstantly)
        {
            _onShowInstantly = onShowInstantly;
            return this;
        }

        public ViewHelper WithOnHideInstantly(Action onHideInstantly)
        {
            _onHideInstantly = onHideInstantly;
            return this;
        }
    }
}