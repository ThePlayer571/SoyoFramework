using System;
using SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar;
using SoyoFramework.Framework.Runtime.Core.Layers;
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

        public void RegisterModel<T>(T model) where T : class, IModel
        {
            RegisterModule(model);
        }

        public void RegisterSystem<T>(T system) where T : class, ISystem
        {
            RegisterModule(system);
        }

        public T GetModel<T>() where T : class, IModel
        {
            return GetModule<T>();
        }

        public T GetSystem<T>() where T : class, ISystem
        {
            return GetModule<T>();
        }

        public IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent
        {
            return _eventSystem.Register<T>(onEvent);
        }

        public void SendEvent<T>() where T : IEvent, new()
        {
            _eventSystem.Call<T>();
        }

        public void SendEvent<T>(in T e) where T : IEvent
        {
            _eventSystem.Call<T>(in e);
        }


        public void RegisterTool<T>(T tool) where T : class, ITool
        {
            RegisterModule(tool);
        }

        public T GetTool<T>() where T : class, ITool
        {
            return GetModule<T>();
        }

        public void RegisterVController<T>(T viewController) where T : class, IVController
        {
            RegisterModule(viewController);
        }

        public T GetVController<T>() where T : class, IVController
        {
            return GetModule<T>();
        }

        public void SendCommand(ICommand command)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (TEMP_EditorPara.AllCommandIgnoreCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                command.Execute(true);
                return;
            }
#endif
            command.AttachedArchitecture = this;
            command.Execute();
        }

        public void SendCommand<TResult>(ICommand<TResult> command, out TResult result)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (TEMP_EditorPara.AllCommandIgnoreCanExecuteCheck)
            {
                command.AttachedArchitecture = this;
                result = command.Execute(true);
                return;
            }
#endif
            command.AttachedArchitecture = this;
            result = command.Execute();
        }

        public CanExecuteResult TrySendCommand(ICommand command)
        {
#if UNITY_EDITOR
            CommandProfiler.CommandSendHook.OnSend(command);
            if (TEMP_EditorPara.AllCommandIgnoreCanExecuteCheck)
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
            if (TEMP_EditorPara.AllCommandIgnoreCanExecuteCheck)
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
    }
}