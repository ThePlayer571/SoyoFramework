using System;
using Cysharp.Threading.Tasks;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces
{
    public interface IViewHelperAsync : IViewHelperBase
    {
        UniTask ShowAsync();
        UniTask HideAsync();
        void ShowInstantly();
        void HideInstantly();
        AsyncViewState CurrentState { get; }
    }

    public interface IViewHelperAsync<in TData> : IViewHelperBase<TData>, IViewHelperAsync
    {
        UniTask ShowAsync(TData data);
        UniTask HideAsync(TData data);
        void ShowInstantly(TData data);
        void HideInstantly(TData data);
    }


    public static class IViewHelperAsyncExtensions
    {
        public static UniTask SetViewAsync(this IViewHelperAsync viewHelper, AsyncViewState targetState)
        {
            return targetState switch
            {
                AsyncViewState.Show => viewHelper.ShowAsync(),
                AsyncViewState.Hide => viewHelper.HideAsync(),
                _ => UniTask.CompletedTask
            };
        }

        public static void SetViewInstantly(this IViewHelperAsync viewHelper, AsyncViewState targetState)
        {
            switch (targetState)
            {
                case AsyncViewState.Show:
                    viewHelper.ShowInstantly();
                    break;
                case AsyncViewState.Hide:
                    viewHelper.HideInstantly();
                    break;
                case AsyncViewState.Hiding:
                case AsyncViewState.Showing:
                default:
                    break;
            }
        }

        public static UniTask ToggleViewAsync(this IViewHelperAsync viewHelper)
        {
            switch (viewHelper.CurrentState)
            {
                case AsyncViewState.Show:
                case AsyncViewState.Showing:
                    return viewHelper.HideAsync();
                case AsyncViewState.Hide:
                case AsyncViewState.Hiding:
                    return viewHelper.ShowAsync();
                default:
                    var currentState = viewHelper.CurrentState;
                    throw new ArgumentOutOfRangeException(nameof(currentState), currentState, $"无法切换至状态{currentState}");
            }
        }

        public static void ToggleViewInstantly(this IViewHelperAsync viewHelper)
        {
            switch (viewHelper.CurrentState)
            {
                case AsyncViewState.Show:
                case AsyncViewState.Showing:
                    viewHelper.HideInstantly();
                    break;
                case AsyncViewState.Hide:
                case AsyncViewState.Hiding:
                    viewHelper.ShowInstantly();
                    break;
                default:
                    var currentState = viewHelper.CurrentState;
                    throw new ArgumentOutOfRangeException(nameof(currentState), currentState, $"无法切换至状态{currentState}");
            }
        }
    }

    public static class IViewHelperAsyncTExtensions
    {
        public static UniTask SetViewAsync<TData>(this IViewHelperAsync<TData> viewHelper, AsyncViewState targetState,
            TData data)
        {
            return targetState switch
            {
                AsyncViewState.Show => viewHelper.ShowAsync(data),
                AsyncViewState.Hide => viewHelper.HideAsync(data),
                _ => UniTask.CompletedTask
            };
        }

        public static void SetViewInstantly<TData>(this IViewHelperAsync<TData> viewHelper, AsyncViewState targetState,
            TData data)
        {
            switch (targetState)
            {
                case AsyncViewState.Show:
                    viewHelper.ShowInstantly(data);
                    break;
                case AsyncViewState.Hide:
                    viewHelper.HideInstantly(data);
                    break;
                case AsyncViewState.Hiding:
                case AsyncViewState.Showing:
                default:
                    break;
            }
        }

        public static UniTask ToggleViewAsync<TData>(this IViewHelperAsync<TData> viewHelper, TData data)
        {
            switch (viewHelper.CurrentState)
            {
                case AsyncViewState.Show:
                case AsyncViewState.Showing:
                    return viewHelper.HideAsync(data);
                case AsyncViewState.Hide:
                case AsyncViewState.Hiding:
                    return viewHelper.ShowAsync(data);
                default:
                    var currentState = viewHelper.CurrentState;
                    throw new ArgumentOutOfRangeException(nameof(currentState), currentState, $"无法切换至状态{currentState}");
            }
        }

        public static void ToggleViewInstantly<TData>(this IViewHelperAsync<TData> viewHelper, TData data)
        {
            switch (viewHelper.CurrentState)
            {
                case AsyncViewState.Show:
                case AsyncViewState.Showing:
                    viewHelper.HideInstantly(data);
                    break;
                case AsyncViewState.Hide:
                case AsyncViewState.Hiding:
                    viewHelper.ShowInstantly(data);
                    break;
                default:
                    var currentState = viewHelper.CurrentState;
                    throw new ArgumentOutOfRangeException(nameof(currentState), currentState, $"无法切换至状态{currentState}");
            }
        }
    }
}