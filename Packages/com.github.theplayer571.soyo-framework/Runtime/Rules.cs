using SoyoFramework.Utils.LogKit;

namespace SoyoFramework
{
    #region 接口：基础规则

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
        ICanRegisterAggregateRoot, ICanUnregisterAggregateRoot
    {
    }

    public interface IViewControllerRule :
        ICanSendCommand, ICanGetAggregateRoot
    {
    }

    public interface ICommandRule :
        ICanSendCommand,
        ICanGetAggregateRoot, ICanRegisterAggregateRoot, ICanUnregisterAggregateRoot
    {
    }

    #endregion

    #region Extensions

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
                if (!AggregateHigherThan.ValidateLifecycle(member.AggregateRoot, typeof(T)))
                {
                    $"Aggregate {member.AggregateRoot.GetType().Name} 不能注册 {typeof(T).Name}：目标不是当前聚合的 HigherThan 下位聚合。"
                        .LogError();
                }
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
                if (!AggregateHigherThan.ValidateLifecycle(member.AggregateRoot, typeof(T)))
                {
                    $"Aggregate {member.AggregateRoot.GetType().Name} 不能注销 {typeof(T).Name}：目标不是当前聚合的 HigherThan 下位聚合。"
                        .LogError();
                }
            }
#endif

            Architecture.Instance.UnregisterAggregateRoot<T>();
        }
    }

    #endregion
}
