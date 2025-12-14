using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoyoFramework.OptionalKits.ActionKit.Runtime
{
    internal partial class ActionChain
    {
        private readonly List<Func<CancellationToken, UniTask>> _actions = new List<Func<CancellationToken, UniTask>>();
        private readonly int _repeatCount;

        internal ActionChain(int repeatCount = 1)
        {
            _repeatCount = Math.Max(1, repeatCount);
        }

        private async UniTask ExecuteOnce(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var action in _actions)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await action(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常的取消操作，不需要处理
            }
        }

        private async UniTask Start(CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < _repeatCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await ExecuteOnce(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常的取消操作，不需要处理
            }
        }
    }

    internal partial class ActionChain : IActionChain
    {
        public IActionChain Delay(float seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
                "Delay time cannot be negative. Set to 0.".LogWarning();
            }

            _actions.Add(async (token) => await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token));
            return this;
        }

        public IActionChain DelayFrame(int frames)
        {
            _actions.Add(async (token) => await UniTask.DelayFrame(frames, cancellationToken: token));
            return this;
        }

        public IActionChain Callback(Action callback)
        {
            _actions.Add((_) =>
            {
                callback?.Invoke();
                return UniTask.CompletedTask;
            });
            return this;
        }

        public IActionChain Callback(Func<UniTask> callback)
        {
            _actions.Add(async (token) =>
            {
                if (callback != null)
                {
                    await callback().AttachExternalCancellation(token);
                }
            });
            return this;
        }

        public IActionChain CallbackWithoutWait(Func<UniTask> callback)
        {
            _actions.Add((_) =>
            {
                callback?.Invoke().Forget();
                return UniTask.CompletedTask;
            });
            return this;
        }

        public IActionChain After(Func<bool> condition)
        {
            _actions.Add(async (token) => await UniTask.WaitUntil(condition, cancellationToken: token));
            return this;
        }

        public IActionChain After(Func<UniTask> task)
        {
            _actions.Add(async (token) =>
            {
                if (task != null)
                {
                    await task().AttachExternalCancellation(token);
                }
            });
            return this;
        }


        public IActionChain AfterCurrentSceneUnloaded()
        {
            _actions.Add(async (token) =>
            {
                var currentScene = SceneManager.GetActiveScene();
                var tcs = new UniTaskCompletionSource();

                void OnSceneUnloaded(Scene scene)
                {
                    if (scene == currentScene)
                    {
                        SceneManager.sceneUnloaded -= OnSceneUnloaded;
                        tcs.TrySetResult();
                    }
                }

                SceneManager.sceneUnloaded += OnSceneUnloaded;

                // 如果当前场景已经被卸载，立即完成
                if (!currentScene.isLoaded)
                {
                    SceneManager.sceneUnloaded -= OnSceneUnloaded;
                    return;
                }

                await tcs.Task.AttachExternalCancellation(token);
            });
            return this;
        }

        public void Start()
        {
            Start(CancellationToken.None).Forget();
        }

        public async UniTask ExecuteAsync()
        {
            await Start(CancellationToken.None);
        }

        public void StartWith(MonoBehaviour mono)
        {
            if (mono == null)
            {
                Start();
                return;
            }

            var cancellationToken = mono.GetCancellationTokenOnDestroy();
            Start(cancellationToken).Forget();
        }

        public async UniTask ExecuteWithAsync(MonoBehaviour mono)
        {
            if (mono == null)
            {
                await ExecuteAsync();
                return;
            }

            var cancellationToken = mono.GetCancellationTokenOnDestroy();
            await Start(cancellationToken);
        }

        public void StartWith(CancellationToken token)
        {
            Start(token).Forget();
        }
        // public IActionChain DOTween(Func<Tween> tweenFunc)
        // {
        //     _actions.Add(async (token) =>
        //     {
        //         var tween = tweenFunc?.Invoke();
        //         if (tween != null && tween.IsActive())
        //         {
        //             await tween.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
        //         }
        //     });
        //     return this;
        // }
    }
}