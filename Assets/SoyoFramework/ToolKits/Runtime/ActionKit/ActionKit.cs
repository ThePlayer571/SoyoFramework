using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SoyoFramework.ToolKits.Runtime.ActionKit
{
    public static class ActionKit
    {
        /// <summary>
        /// 每帧调用onUpdate，返回可用于停止的句柄。可选外部CancelToken。
        /// </summary>
        public static UpdateHandle RunOnUpdate(Action onUpdate, CancellationToken externalToken = default)
        {
            var internalCts = new CancellationTokenSource();
            var linkedCts = externalToken != default
                ? CancellationTokenSource.CreateLinkedTokenSource(internalCts.Token, externalToken)
                : internalCts;

            async UniTaskVoid Loop()
            {
                try
                {
                    while (!linkedCts.IsCancellationRequested)
                    {
                        onUpdate?.Invoke();
                        await UniTask.Yield(PlayerLoopTiming.Update, linkedCts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }

            Loop().Forget();
            return new UpdateHandle(internalCts);
        }
    }
}