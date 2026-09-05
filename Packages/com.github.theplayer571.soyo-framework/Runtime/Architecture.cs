using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;

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
        private readonly TypeEventSystem _eventSystem = new();

        private Architecture()
        {
        }

        #region 接口实现

        public bool IgnoreCommandCanExecuteCheck { get; set; } = false;

        public IUnRegister RegisterEvent<T>(Action<T> onEvent)
        {
            return _eventSystem.Register<T>(onEvent);
        }

        public void SendEvent<T>() where T : new()
        {
            _eventSystem.Call<T>();
        }

        public void SendEvent<T>(in T e)
        {
            _eventSystem.Call<T>(in e);
        }

        public void RegisterAggregateRoot<T>(T aggregateRoot) where T : class, IAggregateRoot
        {
            if (_aggregateRootRegistry.TryRegister(typeof(T), aggregateRoot))
            {
                _eventSystem.Call(new AfterAggregateRootRegistered<T>(aggregateRoot));
            }
        }

        public void UnregisterAggregateRoot<T>() where T : class, IAggregateRoot
        {
            _aggregateRootRegistry.RequestUnregister(typeof(T));
        }

        public T? GetAggregateRoot<T>() where T : class, IAggregateRoot
        {
            return _aggregateRootRegistry.Get(typeof(T)) as T;
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
