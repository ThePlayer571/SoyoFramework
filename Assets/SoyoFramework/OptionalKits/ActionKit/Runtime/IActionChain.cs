using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if DOTWEEN_EXISTS
using DG.Tweening;
#endif

namespace SoyoFramework.OptionalKits.ActionKit.Runtime
{
    public interface IActionChain
    {
        /// <summary>
        /// 按秒延迟
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        IActionChain Delay(float seconds);

        /// <summary>
        /// 按帧延迟
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        IActionChain DelayFrame(int frames);

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IActionChain Callback(Action callback);

        /// <summary>
        /// 执行异步函数，等待其完成
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IActionChain Callback(Func<UniTask> callback);

        /// <summary>
        /// 执行异步函数，但不等待其完成
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IActionChain CallbackWithoutWait(Func<UniTask> callback);

        /// <summary>
        /// 延迟至条件满足
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IActionChain After(Func<bool> condition);

        /// <summary>
        /// 延迟至异步任务完成
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        IActionChain After(Func<UniTask> task);

        /// <summary>
        /// 延迟至当前场景卸载完成
        /// </summary>
        /// <returns></returns>
        IActionChain AfterCurrentSceneUnloaded();

        /// <summary>
        /// 执行该ActionChain（即发即忘）
        /// </summary>
        void Start();

        /// <summary>
        /// 执行该ActionChain（即发即忘），MonoBehaviour销毁时取消
        /// </summary>
        /// <param name="mono"></param>
        void StartWith(MonoBehaviour mono);

        /// <summary>
        /// 执行该ActionChain（即发即忘），支持CancellationToken
        /// </summary>
        /// <param name="token"></param>
        void StartWith(CancellationToken token);

        /// <summary>
        /// 执行该ActionChain
        /// </summary>
        /// <returns>ActionChain执行的异步任务</returns>
        UniTask ExecuteAsync();

        /// <summary>
        ///  执行该ActionChain，MonoBehaviour销毁时取消
        /// </summary>
        /// <param name="mono"></param>
        /// <returns>ActionChain执行的异步任务</returns>
        UniTask ExecuteWithAsync(MonoBehaviour mono);

#if DOTWEEN_EXISTS
        /// 执行DOTween语句，并等待其完成
        /// <summary>
        /// <param name="tweenFunc"></param>
        /// <returns></returns>
        /// </summary>
        IActionChain DOTween(Func<Tween> tweenFunc);
#endif
    }
}