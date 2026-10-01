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
        /// <typeparam name="T"></typeparam>
        void RegisterAggregateRoot<T>(T aggregateRoot) where T : class, IAggregateRoot;

        /// <summary>
        /// 获取对应 key 的 AggregateRoot。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T? GetAggregateRoot<T>() where T : class, IAggregateRoot;

        /// <summary>
        /// 卸载对应 key 的 AggregateRoot。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void UnregisterAggregateRoot<T>() where T : class, IAggregateRoot;

        #endregion

        /// <summary>
        /// 订阅框架级通知。当前用于订阅聚合根注册完成通知。
        /// </summary>
        IUnRegister RegisterEvent<T>(Action<T> onEvent);

        #region Command

        /// <summary>
        /// 是否忽略所有Command的CanExecute检查（仅Editor下生效）
        /// </summary>
        bool IgnoreCommandCanExecuteCheck { get; set; }

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

    public interface IAggregateMember : IAggregateRule
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
        /// 时序：回调时已被移出容器。
        /// </summary>
        protected internal void OnUnregister();
    }

    public interface IViewController : IViewControllerRule
    {
    }

    public interface ICommand : ICommandRule
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