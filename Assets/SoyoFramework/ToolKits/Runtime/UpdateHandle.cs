using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SoyoFramework.ToolKits.Runtime
{
    public struct UpdateHandle : IDisposable
    {
        private CancellationTokenSource _cts;
        private bool _isStopped;

        private UpdateHandle(CancellationTokenSource cts)
        {
            _cts = cts;
            _isStopped = false;
        }

        /// <summary>
        /// 停止Update任务并释放资源
        /// </summary>
        public void Stop()
        {
            if (_isStopped) return;
            _isStopped = true;

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// 与Stop等价，更建议调用Stop以表达意图
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// 开启一个Update任务，在Update生命周期调用onUpdate回调
        /// </summary>
        /// <param name="onUpdate"></param>
        /// <param name="externalToken"></param>
        /// <returns></returns>
        public static UpdateHandle Start(Action onUpdate, CancellationToken externalToken = default)
        {
            var internalCts = new CancellationTokenSource();
            var linkedCts = externalToken != CancellationToken.None
                ? CancellationTokenSource.CreateLinkedTokenSource(internalCts.Token, externalToken)
                : internalCts;

            Loop().Forget();
            return new UpdateHandle(internalCts);

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
        }


        /// <summary>
        /// 开启一个FixedUpdate任务，在FixedUpdate生命周期调用onUpdate回调
        /// </summary>
        /// <param name="onUpdate"></param>
        /// <param name="externalToken"></param>
        /// <returns></returns>
        public static UpdateHandle StartFixed(Action onUpdate, CancellationToken externalToken = default)
        {
            var internalCts = new CancellationTokenSource();
            var linkedCts = externalToken != CancellationToken.None
                ? CancellationTokenSource.CreateLinkedTokenSource(internalCts.Token, externalToken)
                : internalCts;

            Loop().Forget();
            return new UpdateHandle(internalCts);

            async UniTaskVoid Loop()
            {
                try
                {
                    while (!linkedCts.IsCancellationRequested)
                    {
                        onUpdate?.Invoke();
                        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, linkedCts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}