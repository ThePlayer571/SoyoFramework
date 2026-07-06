using System;
using SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;

namespace SoyoFramework.Framework.Runtime.Core
{
    public abstract class Architecture<TArch> : IArchitecture
        where TArch : Architecture<TArch>, new()
    {
        #region 可用字段

        public bool IgnoreCommandCanExecuteCheck { get; set; } = false;

        public static TArch Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 语法糖：如果访问Architecture实例时尚未Init，则自动Init
                    Init();
                }

                return _instance;
            }
        }

        public bool Inited => _inited;

        private bool _inited;
        private static TArch _instance;
        private readonly SimpleIOCContainer _container = new();
        private readonly TypeEventSystem _eventSystem = new();
        private readonly ILog _logger = new PrefixLogger($"[{typeof(TArch).Name}]");

        #endregion

        #region 生命周期

        public static void Init(bool setAsDefault = true)
        {
            if (_instance != null)
            {
                "Architecture单例已存在，无法再次Init".LogError();
                return;
            }

            var arch = new TArch();
            _instance = arch;

            // 注册初始层级信息
            arch.OnInit();

            // 初始化模块
            foreach (var module in arch._container.GetAll<IModule>())
            {
                module.PreInit();
            }

            foreach (var module in arch._container.GetAll<IModule>())
            {
                module.Init();
            }

            // 标记已初始化
            arch._inited = true;

            // 语法糖：默认Architecture实例
            if (setAsDefault)
            {
                DefaultArchitecture.Instance = arch;
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

            // 销毁模块
            foreach (var module in arch._container.GetAll<IModule>())
            {
                module.Deinit();
            }

            arch._container.Clear();

            // TypeEventSystem清理
            arch._eventSystem.Clear();

            // 标记未初始化
            _instance = null;


            // 语法糖：默认Architecture实例
            if (DefaultArchitecture.Instance == arch)
            {
                DefaultArchitecture.Instance = null;
            }
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();

        #endregion

        #region 接口实现

        public void RegisterModule<T>(T module) where T : class, IModule
        {
            if (module == null)
            {
                $"注册失败，注册的模块不能为null: {typeof(T).Name}".LogError();
                return;
            }

            module.AttachedArchitecture = this;
            _container.Register<T>(module);

            // 如果是在Architecture初始化后注册的Module，直接初始化
            if (Inited)
            {
                module.PreInit();
                module.Init();
            }
        }

        public T GetModule<T>() where T : class, IModule
        {
            var module = _container.Get<T>();
            if (module == null)
            {
                $"尝试获取未注册的模块: {typeof(T).Name}".LogError();
                return null;
            }

            return module;
        }

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

        public void RegisterDomainRoot<T>(T domainRoot) where T : IDomainRoot
        {
            if (domainRoot == null)
            {
                $"注册失败，注册的DomainRoot不能为null: {typeof(T).Name}".LogError();
                return;
            }

            _container.Register<T>(domainRoot);
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

        public T GetDomainRoot<T>() where T : class, IDomainRoot
        {
            var domainRoot = _container.Get<T>();
            if (domainRoot == null)
            {
                return null;
            }

            return domainRoot;
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
                command.Execute(true);
                return;
            }
#endif
            command.AttachedArchitecture = this;
            command.Execute();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                return command.Execute(true);
            }
#endif
            command.AttachedArchitecture = this;
            return command.Execute();
        }


        public CanExecuteResult TrySendCommand(ICommand command)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                command.Execute(true);
                return CanExecuteResult.Success;
            }
#endif
            command.AttachedArchitecture = this;
            var canExecuteResult = command.CanExecute();
            if (canExecuteResult.CanExecute)
            {
                command.Execute(true);
            }

            return canExecuteResult;
        }

        public CanExecuteResult TrySendCommand<TResult>(ICommand<TResult> command, out TResult result)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (IgnoreCommandCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                result = command.Execute(true);
                return CanExecuteResult.Success;
            }
#endif
            command.AttachedArchitecture = this;
            var canExecute = command.CanExecute();
            if (canExecute.CanExecute)
            {
                result = command.Execute(true);
            }
            else
            {
                result = default;
            }

            return canExecute;
        }

        #endregion

        #region Protected 子类可用

        protected void RegisterVController<T>(T viewController) where T : class, IVController
        {
            RegisterModule(viewController);
        }

        protected void RegisterDomainService<T>(T domainService) where T : class, IDomainService
        {
            RegisterModule(domainService);
        }

        #endregion
    }
}