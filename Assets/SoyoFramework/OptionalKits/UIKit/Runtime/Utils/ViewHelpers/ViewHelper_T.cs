using System;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers
{
    public sealed class ViewHelper<TData> : ViewHelperBase<TData>, IViewHelper<TData>
    {
        #region 操作

        public void Show(TData data)
        {
            if (_onShow == null)
            {
                Logger.LogError("调用Show失败，原因是未设置onShow回调");
                return;
            }

            if (_isVisible) return;

            _onShow.Invoke(data);
            _isVisible = true;
        }

        public void Hide(TData data)
        {
            if (_onHide == null)
            {
                Logger.LogError("调用Hide失败，原因是未设置onHide回调");
                return;
            }

            if (!_isVisible) return;

            _onHide.Invoke(data);
            _isVisible = false;
        }

        public void Show()
        {
            Show(default);
        }

        public void Hide()
        {
            Hide(default);
        }

        public override void Update(TData data)
        {
            if (_onUpdate == null)
            {
                Logger.LogError("调用Update失败，原因是未设置onUpdate回调");
                return;
            }

            if (!_isVisible) return;

            _onUpdate.Invoke(data);
        }

        public override void ForceResetView(TData data)
        {
            if (_isVisible)
            {
                if (_onShow == null)
                {
                    Logger.LogError("调用ForceResetView失败，原因是未设置onShow回调");
                }
                else
                {
                    _onShow.Invoke(data);
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
                    _onHide.Invoke(data);
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

        public ViewHelper<TData> WithOnShow(Action<TData> onShow)
        {
            _onShow = onShow;
            return this;
        }

        public ViewHelper<TData> WithOnShow(Action onShow)
        {
            _onShow = _ => onShow.Invoke();
            return this;
        }

        public ViewHelper<TData> WithOnHide(Action<TData> onHide)
        {
            _onHide = onHide;
            return this;
        }

        public ViewHelper<TData> WithOnHide(Action onHide)
        {
            _onHide = _ => onHide.Invoke();
            return this;
        }

        public ViewHelper<TData> WithOnUpdate(Action<TData> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public ViewHelper<TData> WithOnUpdate(Action onUpdate)
        {
            _onUpdate = _ => onUpdate.Invoke();
            return this;
        }

        #endregion

        // 回调
        private Action<TData> _onShow;
        private Action<TData> _onHide;
        private Action<TData> _onUpdate;

        // 变量
        private bool _isVisible;
    }
}