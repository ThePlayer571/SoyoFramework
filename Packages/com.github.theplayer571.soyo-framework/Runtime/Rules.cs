using System;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    #region 接口：基础规则

    public interface ICanRegisterEvent
    {
    }

    public interface ICanSendEvent
    {
    }

    public interface ICanSendCommand
    {
    }

    public interface ICanRegisterAggregateRoot
    {
    }

    public interface ICanGetAggregateRoot
    {
    }

    public interface ICanUnregisterAggregateRoot
    {
    }

    #endregion

    #region 接口：层级规则

    public interface IAggregateRule :
        ICanRegisterEvent, ICanSendEvent,
        ICanRegisterAggregateRoot, ICanUnregisterAggregateRoot
    {
    }

    public interface IViewControllerRule :
        ICanRegisterEvent, ICanSendCommand, ICanGetAggregateRoot
    {
    }

    public interface ICommandRule :
        ICanSendEvent, ICanSendCommand,
        ICanGetAggregateRoot, ICanRegisterAggregateRoot, ICanUnregisterAggregateRoot
    {
    }

    #endregion

    #region Extensions

    public static class CanRegisterEventExtension
    {
        /// <summary>
        /// 按类型注册事件，会在事件发送时调用onEvent。返回一个IUnRegister，调用它可以取消注册
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
            => Architecture.Instance.RegisterEvent<T>(onEvent);
    }

    public static class CanSendEventExtension
    {
        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。这是无需传入实例的语法糖，T必须包含无参构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SendEvent<T>(this ICanSendEvent self) where T : new()
            => Architecture.Instance.SendEvent<T>();

        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        public static void SendEvent<T>(this ICanSendEvent self, T e)
            => Architecture.Instance.SendEvent<T>(e);
    }

    public static class CanSendCommandExtension
    {
        /// <summary>
        /// 发送一个Command
        /// </summary>
        /// <param name="self"></param>
        /// <param name="command"></param>
        public static void SendCommand(this ICanSendCommand self, ICommand command)
            => Architecture.Instance.SendCommand(command);

        /// <summary>
        /// 发送一个Command，并获取返回值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="command"></param>
        /// <typeparam name="TResult"></typeparam>
        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command)
            => Architecture.Instance.SendCommand(command);


        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult
        /// </summary>
        /// <param name="self"></param>
        /// <param name="command"></param>
        /// <param name="canExecuteResult"></param>
        /// <returns></returns>
        public static bool TrySendCommand(this ICanSendCommand self,
            ICommand command, out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out canExecuteResult);


        /// <summary>
        /// 尝试发送一个Command
        /// </summary>
        /// <param name="self"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool TrySendCommand(this ICanSendCommand self, ICommand command)
            => Architecture.Instance.TrySendCommand(command, out _);


        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult和返回值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <param name="canExecuteResult"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static bool TrySendCommand<TResult>(this ICanSendCommand self,
            ICommand<TResult> command, out TResult? result, out CanExecuteResult canExecuteResult)
            => Architecture.Instance.TrySendCommand(command, out result, out canExecuteResult);

        /// <summary>
        /// 尝试发送一个Command，并返回返回值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <param name="canExecuteResult"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static bool TrySendCommand<TResult>(this ICanSendCommand self,
            ICommand<TResult> command, out TResult? result)
            => Architecture.Instance.TrySendCommand(command, out result, out _);
    }

    public static class CanRegisterAggregateRootExtension
    {
        /// <summary>
        /// 注册一个AggregateRoot。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="self"></param>
        /// <param name="aggregateRoot"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterAggregateRoot<T>(this ICanRegisterAggregateRoot self, T aggregateRoot)
            where T : class, IAggregateRoot
        {
#if UNITY_EDITOR
            // AggregateMember 检验
            if (self is IAggregateMember member)
            {
                AggregateHigherThan.ValidateLifecycle(
                    member.AggregateRoot,
                    typeof(T),
                    AggregateLifecycleOperation.Register);
            }
#endif

            Architecture.Instance.RegisterAggregateRoot(aggregateRoot);
        }
    }

    public static class CanGetAggregateRootExtension
    {
        /// <summary>
        /// 获取一个AggregateRoot。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetAggregateRoot<T>(this ICanGetAggregateRoot self)
            where T : class, IAggregateRoot
        {
            return Architecture.Instance.GetAggregateRoot<T>();
        }
    }

    public static class CanUnregisterAggregateRootExtension
    {
        /// <summary>
        /// 卸载一个AggregateRoot。会以T为key从IOCContainer里移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UnregisterAggregateRoot<T>(this ICanUnregisterAggregateRoot self)
            where T : class, IAggregateRoot
        {
#if UNITY_EDITOR
            // AggregateMember 检验
            if (self is IAggregateMember member)
            {
                AggregateHigherThan.ValidateLifecycle(
                    member.AggregateRoot,
                    typeof(T),
                    AggregateLifecycleOperation.Unregister);
            }
#endif

            Architecture.Instance.UnregisterAggregateRoot<T>();
        }
    }

    #endregion
}
