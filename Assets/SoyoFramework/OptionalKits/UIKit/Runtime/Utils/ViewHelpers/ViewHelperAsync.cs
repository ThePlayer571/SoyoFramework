using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers
{
    public sealed class ViewHelperAsync : ViewHelperBase, IViewHelperAsync
    {
        #region 操作

        public async UniTask ShowAsync()
        {
            if (_onShowAsync == null)
            {
                Logger.LogInfo("调用ShowAsync失败，原因是未设置onShowAsync回调，自动回退到ShowInstantly");
                ShowInstantly();
                return;
            }

            // 提前退出：目标动画本就在进行
            if (_state == AsyncViewState.Show || _state == AsyncViewState.Showing)
                return;

            // 中断动画：如果在进行其他动画
            if (_state == AsyncViewState.Hiding)
            {
                TryInterruptCurrentAnimationInternal();
            }

            // 开始动画：播放目标动画
            _state = AsyncViewState.Showing;
            try
            {
                _cts = new CancellationTokenSource();
                await _onShowAsync.Invoke(_cts.Token);
                _state = AsyncViewState.Show;
            }
            catch (OperationCanceledException)
            {
                Logger.LogInfo("ShowAsync动画被中断，通过默认处理解决");
            }
            catch (Exception ex)
            {
                Logger.LogError($"ShowAsync动画出现异常：{ex}");
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        public async UniTask HideAsync()
        {
            if (_onHideAsync == null)
            {
                Logger.LogInfo("调用HideAsync失败，原因是未设置onHideAsync回调，自动回退到HideInstantly");
                HideInstantly();
                return;
            }

            // 提前退出：目标动画本就在进行
            if (_state == AsyncViewState.Hide || _state == AsyncViewState.Hiding)
                return;

            // 中断动画：如果在进行其他动画
            if (_state == AsyncViewState.Showing)
            {
                TryInterruptCurrentAnimationInternal();
            }

            // 开始动画：播放目标动画
            _state = AsyncViewState.Hiding;
            try
            {
                _cts = new CancellationTokenSource();
                await _onHideAsync.Invoke(_cts.Token);
                _state = AsyncViewState.Hide;
            }
            catch (OperationCanceledException)
            {
                Logger.LogInfo("HideAsync动画被中断，通过默认处理解决");
            }
            catch (Exception ex)
            {
                Logger.LogError($"HideAsync动画出现异常：{ex}");
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        public void ShowInstantly()
        {
            if (_onShowInstantly == null)
            {
                Logger.LogError("调用ShowInstantly失败，原因是未设置onShowInstantly回调");
                return;
            }

            // 提前退出：目标动画本就在进行
            if (_state == AsyncViewState.Show)
                return;

            // 中断动画：如果在进行其他动画
            if (_state == AsyncViewState.Hiding || _state == AsyncViewState.Showing)
            {
                TryInterruptCurrentAnimationInternal();
            }

            // 直接显示
            _onShowInstantly.Invoke();
            _state = AsyncViewState.Show;
        }

        public void HideInstantly()
        {
            if (_onHideInstantly == null)
            {
                Logger.LogError("调用HideInstantly失败，原因是未设置onHideInstantly回调");
                return;
            }

            // 提前退出：目标动画本就在进行
            if (_state == AsyncViewState.Hide)
                return;

            // 中断动画：如果在进行其他动画
            if (_state == AsyncViewState.Showing || _state == AsyncViewState.Hiding)
            {
                TryInterruptCurrentAnimationInternal();
            }

            // 直接隐藏
            _onHideInstantly.Invoke();
            _state = AsyncViewState.Hide;
        }


        public override void Update()
        {
            if (_onUpdate == null)
            {
                Logger.LogError("调用Update失败，原因是未设置OnUpdate回调");
                return;
            }

            // 提前退出：不显示，更新无意义
            if (_state == AsyncViewState.Hide)
                return;

            _onUpdate.Invoke();
        }

        public override void ForceResetView()
        {
            // 强制中断一切动画
            TryInterruptCurrentAnimationInternal();

            switch (_state)
            {
                case AsyncViewState.Show:
                {
                    if (_onShowInstantly == null)
                    {
                        Logger.LogError("调用ForceResetView失败，原因是未设置onShowInstantly回调");
                        return;
                    }

                    _onShowInstantly.Invoke();
                    break;
                }
                case AsyncViewState.Hide:
                {
                    if (_onHideInstantly == null)
                    {
                        Logger.LogError("调用ForceResetView失败，原因是未设置onHideInstantly回调");
                        return;
                    }

                    _onHideInstantly.Invoke();
                    break;
                }
                case AsyncViewState.Showing:
                case AsyncViewState.Hiding:
                default:
                    throw new NotSupportedException($"无法ForceResetView到当前状态: {_state}");
            }
        }

        #endregion

        #region 构造

        public ViewHelperAsync(AsyncViewState initialState = AsyncViewState.Hide)
        {
            _state = initialState;
            ShowMoreLog = false;
        }

        public ViewHelperAsync WithOnShow(Func<CancellationToken, UniTask> onShowAsync)
        {
            _onShowAsync = onShowAsync;
            return this;
        }

        public ViewHelperAsync WithOnHide(Func<CancellationToken, UniTask> onHideAsync)
        {
            _onHideAsync = onHideAsync;
            return this;
        }

        public ViewHelperAsync WithOnUpdate(Action onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public ViewHelperAsync WithOnShowInstantly(Action onShowInstantly)
        {
            _onShowInstantly = onShowInstantly;
            return this;
        }

        public ViewHelperAsync WithOnHideInstantly(Action onHideInstantly)
        {
            _onHideInstantly = onHideInstantly;
            return this;
        }

        #endregion

        #region 属性

        public bool ShowMoreLog
        {
            get => Logger.LogStrategy == LogStrategy.All;
            set => Logger.LogStrategy = value ? LogStrategy.All : LogStrategy.ErrorOnly;
        }

        public AsyncViewState CurrentState => _state;

        #endregion

        // 内部函数：中断动画
        private void TryInterruptCurrentAnimationInternal()
        {
            _cts?.Cancel();
        }

        // 回调
        private Func<CancellationToken, UniTask> _onShowAsync;
        private Func<CancellationToken, UniTask> _onHideAsync;
        private Action _onUpdate;

        private Action _onShowInstantly;
        private Action _onHideInstantly;

        // 变量
        private AsyncViewState _state;
        private CancellationTokenSource _cts;
    }
}