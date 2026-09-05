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

        #region Event

        /// <summary>
        /// 按类型注册事件，会在事件发送时调用onEvent。返回一个IUnRegister，调用它可以取消注册
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IUnRegister RegisterEvent<T>(Action<T> onEvent);

        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。这是无需传入实例的语法糖，T必须包含无参构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void SendEvent<T>() where T : new();

        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。
        /// </summary>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        void SendEvent<T>(in T e);

        #endregion

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
        /// <summary>
        /// 聚合根注销回调。回调时已被移出容器。
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
