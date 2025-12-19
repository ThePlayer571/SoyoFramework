using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;


namespace SoyoFramework.OptionalKits.ActionKit.Runtime
{
    public static partial class ActionKit
    {
        public static IActionChain Chain()
        {
            return new ActionChain();
        }

        public static IActionChain ChainRepeat(int repeatCount)
        {
            if (repeatCount < 1)
                throw new ArgumentException("重复次数不得小于1", nameof(repeatCount));

            return new ActionChain(repeatCount);
        }

        public static IUnRegister RunOnUpdate(Action callback)
        {
            if (callback == null)
                return new CustomUnRegister(() => { }); // 返回空的UnRegister

            // 如果已经在运行，先停止
            if (_updateCallbacks.ContainsKey(callback))
            {
                StopRunOnUpdate(callback);
            }

            var cts = new CancellationTokenSource();
            _updateCallbacks[callback] = cts;

            // 启动更新循环
            RunUpdateLoop(callback, cts.Token).Forget();

            // 返回IUnRegister实例
            return new CustomUnRegister(() => StopRunOnUpdate(callback));
        }

        public static void StopRunOnUpdate(Action callback)
        {
            if (callback == null) return;

            if (_updateCallbacks.TryGetValue(callback, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
                _updateCallbacks.Remove(callback);
            }
        }
    }

    public static partial class ActionKit
    {
        private static readonly Dictionary<Action, CancellationTokenSource> _updateCallbacks = new();

        private static async UniTaskVoid RunUpdateLoop(Action callback, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    callback?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                // 正常的取消操作，不需要处理
            }
            catch (Exception ex)
            {
                Debug.LogError($"RunOnUpdate callback error: {ex}");
            }
        }
    }
}