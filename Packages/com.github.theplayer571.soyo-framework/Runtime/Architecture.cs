using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework
{
    public sealed class Architecture : IArchitecture
    {
        #region 静态接口

        public static IArchitecture Instance
        {
            get
            {
                Init();
                return _instance!;
            }
        }

        public static void Init()
        {
            _instance ??= new Architecture();
        }

        #endregion

        private static IArchitecture? _instance;
        private readonly AggregateRootRegistry _aggregateRootRegistry = new();

        private Architecture()
        {
        }

        #region 接口实现

        public bool RegisterAggregateRoot<T>(T aggregateRoot, IAggregateMember? requester = null)
            where T : class, IAggregateRoot
        {
            return _aggregateRootRegistry.TryRegister(aggregateRoot, requester);
        }

        public bool UnregisterAggregateRoot<T>(IAggregateMember? requester = null) where T : class, IAggregateRoot
        {
            return _aggregateRootRegistry.RequestUnregister<T>(requester);
        }

        public IAggregateRootHandle<T>? GetAggregateRootHandle<T>(IAggregateMember? requester = null)
            where T : class, IAggregateRoot
        {
            return _aggregateRootRegistry.GetHandle<T>(requester);
        }

        public T? GetAggregateRoot<T>(IAggregateMember? requester = null) where T : class, IAggregateRoot
        {
            return _aggregateRootRegistry.GetRoot<T>(requester);
        }

        public void SendCommand(ICommand command)
        {
            var canExecute = command.CanExecute();
            if (!canExecute)
            {
                $"{nameof(ICommand)} {command.GetType().Name} 执行失败，原因: {canExecute.FailMessage}".LogError();
                return;
            }

            command.Execute();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
            var canExecute = command.CanExecute();
            if (!canExecute)
            {
                throw new InvalidOperationException(
                    $"{nameof(ICommand)} {command.GetType().Name} 执行失败，原因: {canExecute.FailMessage}");
            }

            return command.Execute();
        }

        public bool TrySendCommand(ICommand command, out CanExecuteResult canExecuteResult)
        {
            canExecuteResult = command.CanExecute();
            if (canExecuteResult.CanExecute)
            {
                command.Execute();
            }

            return canExecuteResult;
        }

        public bool TrySendCommand<TResult>(ICommand<TResult> command, out TResult? result,
            out CanExecuteResult canExecuteResult)
        {
            canExecuteResult = command.CanExecute();
            if (canExecuteResult.CanExecute)
            {
                result = command.Execute();
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        #endregion
    }
}