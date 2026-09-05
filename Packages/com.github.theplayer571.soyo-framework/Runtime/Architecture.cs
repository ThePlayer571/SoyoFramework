using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    public abstract class Architecture<TArch> : IArchitecture
        where TArch : Architecture<TArch>, new()
    {
        #region 接口实现

        public bool Inited => _inited;
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

        public T InitCommand<T>(T command) where T : ICommand
        {
            command.AttachedArchitecture = this;
            return command;
        }

        public void SendCommand(ICommand command)
        {
            command.AttachedArchitecture = this;

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
            command.AttachedArchitecture = this;

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
            command.AttachedArchitecture = this;

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
            command.AttachedArchitecture = this;

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

        #region 静态接口

        public static TArch Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 语法糖：如果访问Architecture实例时尚未Init，则自动Init
                    Init();
                }

                return _instance!;
            }
        }

        #endregion

        // 静态变量
        private static TArch? _instance;

        // 变量
        private bool _inited;
        private readonly AggregateRootRegistry _aggregateRootRegistry = new();
        private readonly TypeEventSystem _eventSystem = new();

        #region 生命周期

        // [MemberNotNull(nameof(_instance))]
        public static void Init(bool isGlobal = true)
        {
            if (_instance != null)
            {
                $"Architecture单例已存在，无法再次{nameof(Init)}".LogError();
                return;
            }

            var arch = new TArch();
            _instance = arch;

            // 初始化
            arch.OnInit();

            // 标记已初始化
            arch._inited = true;

            // 语法糖：GlobalArchitecture实例
            if (isGlobal)
            {
                GlobalArchitecture.Instance = arch;
            }
        }

        public static void Deinit()
        {
            var arch = _instance;

            if (arch == null)
            {
                $"Architecture单例不存在，无法{nameof(Deinit)}".LogError();
                return;
            }

            if (!arch._aggregateRootRegistry.BeginDeinitialization())
            {
                return;
            }

            var completed = false;
            try
            {
                arch.OnDeinit();

                // 销毁 AggregateRoot。Registry 会建立快照并统一进入串行注销队列。
                arch._aggregateRootRegistry.UnregisterAll();

                // TypeEventSystem清理
                arch._eventSystem.Clear();

                // 标记未初始化
                _instance = null;

                // 语法糖：GlobalArchitecture实例
                if (GlobalArchitecture.Instance == arch)
                {
                    GlobalArchitecture.Instance = null;
                }

                arch._aggregateRootRegistry.CompleteDeinitialization();
                completed = true;
            }
            finally
            {
                // 失败时恢复标志，避免架构永久停留在“正在销毁”状态，
                // 同时保留原始异常向外传播。
                if (!completed)
                {
                    arch._aggregateRootRegistry.AbortDeinitialization();
                }
            }
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();

        #endregion
    }
}
