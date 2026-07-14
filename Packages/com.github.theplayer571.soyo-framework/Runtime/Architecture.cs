using System;
using System.Diagnostics.CodeAnalysis;
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

        public void RegisterDomainRoot<T>(T domainRoot) where T : class, IDomainRoot
        {
            _container.Register<T>(domainRoot);
            _eventSystem.Call(new AfterDomainRootRegistered<T>(domainRoot));
        }

        public void UnregisterDomainRoot<T>() where T : class, IDomainRoot
        {
            var domainRoot = _container.Get<T>();
            if (domainRoot == null)
            {
                $"尝试注销未注册的DomainRoot: {typeof(T).Name}".LogError();
                return;
            }

            domainRoot.Deinit();
            _container.Unregister<T>();
        }

        public T? GetDomainRoot<T>() where T : class, IDomainRoot
        {
            return _container.Get<T>();
        }

        public T InitCommand<T>(T command) where T : ICommand
        {
            command.AttachedArchitecture = this;
            return command;
        }

        public void SendCommand(ICommand command)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                command.Execute();
                return;
            }
#endif

            command.AttachedArchitecture = this;

            var canExecute = command.CanExecute();
            if (!canExecute)
            {
                $"Command {command.GetType().Name} 执行失败，原因: {canExecute.FailMessage}".LogError();
                return;
            }

            command.Execute();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                return command.Execute();
            }
#endif
            command.AttachedArchitecture = this;

            var canExecute = command.CanExecute();
            if (!canExecute)
            {
                throw new InvalidOperationException(
                    $"Command {command.GetType().Name} 执行失败，原因: {canExecute.FailMessage}");
            }

            return command.Execute();
        }

        public bool TrySendCommand(ICommand command, out CanExecuteResult canExecuteResult)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                command.Execute();
                canExecuteResult = CanExecuteResult.Success;
                return true;
            }
#endif
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
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                result = command.Execute();
                canExecuteResult = CanExecuteResult.Success;
                return true;
            }
#endif
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
        private readonly SimpleIOCContainer _container = new();
        private readonly TypeEventSystem _eventSystem = new();
        private readonly ILog _logger = new PrefixLogger($"[{typeof(TArch).Name}]");

        #region 生命周期

        // [MemberNotNull(nameof(_instance))]
        public static void Init(bool isGlobal = true)
        {
            if (_instance != null)
            {
                "Architecture单例已存在，无法再次Init".LogError();
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
                "Architecture单例不存在，无法Deinit".LogError();
                return;
            }

            arch.OnDeinit();

            // 销毁 DomainRoot
            foreach (var domainRoot in arch._container.GetAll<IDomainRoot>())
            {
                domainRoot.Deinit();
            }

            arch._container.Clear();

            // TypeEventSystem清理
            arch._eventSystem.Clear();

            // 标记未初始化
            _instance = null;

            // 语法糖：GlobalArchitecture实例
            if (GlobalArchitecture.Instance == arch)
            {
                GlobalArchitecture.Instance = null;
            }
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();

        #endregion
    }
}