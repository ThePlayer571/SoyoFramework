using System;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    public interface IArchitecture
    {
        #region AggregateRoot

        /// <summary>
        /// 注册一个 AggregateRoot。
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <param name="requester"></param>
        /// <typeparam name="T"></typeparam>
        bool RegisterAggregateRoot<T>(T aggregateRoot, IAggregateMember? requester = null)
            where T : class, IAggregateRoot;

        /// <summary>
        /// 获取对应 key 的 AggregateRootHandle。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAggregateRootHandle<T>? GetAggregateRootHandle<T>(IAggregateMember? requester = null)
            where T : class, IAggregateRoot;

        T? GetAggregateRoot<T>(IAggregateMember? requester = null) where T : class, IAggregateRoot;

        /// <summary>
        /// 卸载对应 key 的 AggregateRoot。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        bool UnregisterAggregateRoot<T>(IAggregateMember? requester = null) where T : class, IAggregateRoot;

        #endregion

        #region Command

        /// <summary>
        /// 发送一个Command
        /// </summary>
        /// <param name="command"></param>
        void SendCommand(ICommand command);

        /// <summary>
        /// 发送一个Command，并获取返回值
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="TResult"></typeparam>
        TResult SendCommand<TResult>(ICommand<TResult> command);

        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult
        /// </summary>
        /// <param name="command"></param>
        /// <param name="canExecuteResult"></param>
        /// <returns></returns>
        bool TrySendCommand(ICommand command, out CanExecuteResult canExecuteResult);

        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult和返回值
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <param name="canExecuteResult"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        bool TrySendCommand<TResult>(
            ICommand<TResult> command,
            out TResult? result,
            out CanExecuteResult canExecuteResult);

        #endregion
    }

    public interface IAggregateRootHandle<T> where T : class, IAggregateRoot
    {
        T? Current { get; }

        bool TryGet(out T? aggregateRoot);

        IUnRegister RegisterChanged(Action<T?> onChanged);
    }

    public interface IAggregateMember
    {
        IAggregateRoot AggregateRoot { get; }

    }

    public interface IAggregateRoot : IAggregateMember
    {
        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        protected internal void SendEvent<T>() where T : new();
        protected internal void SendEvent<T>(in T e);

        /// <summary>
        /// 聚合根注销回调。
        /// 时序：回调成功返回后，聚合根才移出注册表。
        /// </summary>
        protected internal void OnUnregister();
    }

    public interface IViewController
    {
    }

    public interface ICommand
    {
        /// <summary>
        /// 执行Command的逻辑，约定只通过Architecture来调用
        /// </summary>
        protected internal void Execute();

        CanExecuteResult CanExecute();
    }

    public interface ICommand<out TResult> : ICommand
    {
        /// <summary>
        /// 执行Command的逻辑，约定只通过Architecture来调用
        /// </summary>
        protected internal new TResult Execute();
    }
}
