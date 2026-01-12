using System;
using SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers
{
    public sealed class ViewHelper : ViewHelperBase, IViewHelper
    {
        #region 操作

        public void Show()
        {
            if (_onShow == null)
            {
                Logger.LogError("调用Show失败，原因是未设置onShow回调");
                return;
            }

            if (_isVisible) return;

            _onShow.Invoke();
            _isVisible = true;
        }

        public void Hide()
        {
            if (_onHide == null)
            {
                Logger.LogError("调用Hide失败，原因是未设置onHide回调");
                return;
            }

            if (!_isVisible) return;

            _onHide.Invoke();
            _isVisible = false;
        }


        public override void Update()
        {
            if (_onUpdate == null)
            {
                Logger.LogError("调用Update失败，原因是未设置onUpdate回调");
                return;
            }

            if (!_isVisible) return;

            _onUpdate.Invoke();
        }

        public override void ForceResetView()
        {
            if (_isVisible)
            {
                if (_onShow == null)
                {
                    Logger.LogError("调用ForceResetView失败，原因是未设置onShow回调");
                }
                else
                {
                    _onShow.Invoke();
                }
            }
            else
            {
                if (_onHide == null)
                {
                    Logger.LogError("调用ForceResetView失败，原因是未设置onHide回调");
                }
                else
                {
                    _onHide.Invoke();
                }
            }
        }

        #endregion

        #region 属性
        
        public bool IsShow => _isVisible;

        #endregion

        #region 构造

        public ViewHelper(bool initialVisible = false)
        {
            _isVisible = initialVisible;
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

        public ViewHelper WithOnUpdate(Action onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        #endregion

        // 回调
        private Action _onShow;
        private Action _onHide;
        private Action _onUpdate;

        // 变量
        private bool _isVisible;
    }
}