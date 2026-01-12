using System;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Core.Layers;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core
{
    public abstract class AbstractArchitecture : IArchitecture
    {
        #region 可用字段

        public bool Inited { get; private set; } = false;

        public static IArchitecture Instance { get; private set; }
        private readonly SimpleIOCContainer _container = new();
        private readonly TypeEventSystem _eventSystem = new();

        #endregion

        #region 生命周期

        public void Init(bool setAsDefault = true)
        {
            if (Inited)
            {
                Debug.LogError("Architecture已经初始化");
                return;
            }

            // 单例支持
            Instance = this;

            // 注册初始层级信息
            OnInit();

            // 初始化模块
            foreach (var module in _container.GetAll<IModule>())
            {
                module.PreInit();
            }

            foreach (var module in _container.GetAll<IModule>())
            {
                module.Init();
            }

            //
            Inited = true;

            // ArchitectureHelper支持
            if (setAsDefault)
            {
                ArchitectureHelper.DefaultArchitecture = this;
            }
        }

        public void Deinit()
        {
            // 防止重复Deinit
            if (!Inited)
            {
                Debug.LogError("Architecture未初始化，无法Deinit");
                return;
            }

            OnDeinit();

            // 销毁模块
            foreach (var module in _container.GetAll<IModule>())
            {
                module.Deinit();
            }

            // TypeEventSystem清理
            _eventSystem.Clear();

            // 标记未初始化
            Inited = false;

            // ArchitectureHelper支持
            if (ArchitectureHelper.DefaultArchitecture == this)
            {
                ArchitectureHelper.DefaultArchitecture = null;
            }

            // 单例支持
            Instance = null;
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();

        #endregion

        #region 接口实现

        public void RegisterModule<T>(T module) where T : class, IModule
        {
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
            return _container.Get<T>();
        }

        public void RegisterModel<T>(T model) where T : class, IModel
        {
            model.AttachedArchitecture = this;
            _container.Register<T>(model);

            // 如果是在Architecture初始化后注册的Module，直接初始化
            if (Inited)
            {
                model.PreInit();
                model.Init();
            }
        }

        public void RegisterSystem<T>(T system) where T : class, ISystem
        {
            system.AttachedArchitecture = this;
            _container.Register<T>(system);

            // 如果是在Architecture初始化后注册的Module，直接初始化
            if (Inited)
            {
                system.PreInit();
                system.Init();
            }
        }


        public T GetSystem<T>() where T : class, ISystem
        {
            return _container.Get<T>();
        }

        public T GetModel<T>() where T : class, IModel
        {
            return _container.Get<T>();
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
            tool.AttachedArchitecture = this;
            _container.Register<T>(tool);

            // 如果是在Architecture初始化后注册的Module，直接初始化
            if (Inited)
            {
                tool.PreInit();
                tool.Init();
            }
        }

        public T GetTool<T>() where T : class, ITool
        {
            return _container.Get<T>();
        }

        public void RegisterVController<T>(T viewController) where T : class, IVController
        {
            viewController.AttachedArchitecture = this;
            _container.Register<T>(viewController);

            // 如果是在Architecture初始化后注册的Module，直接初始化
            if (Inited)
            {
                viewController.PreInit();
                viewController.Init();
            }
        }

        public T GetVController<T>() where T : class, IVController
        {
            return _container.Get<T>();
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