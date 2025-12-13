using System;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public abstract class AbstractArchitecture : IArchitecture
    {
        #region 可用字段

        public bool Inited { get; private set; } = false;
        private readonly SimpleIOCContainer _container = new();
        private readonly TypeEventSystem _eventSystem = new();

        #endregion

        #region 生命周期

        public void Init(bool setAsDefault = true)
        {
            if (Inited)
            {
                "Architecture已经初始化".LogError();
                return;
            }

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
                "Architecture没有初始化，无法Deinit".LogError();
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
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();

        #endregion

        #region 接口实现

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

        public void RegisterViewController<T>(T viewController) where T : class, IViewController
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

        public T GetViewController<T>() where T : class, IViewController
        {
            return _container.Get<T>();
        }

        public void SendCommand(ICommand command)
        {
            command.AttachedArchitecture = this;
            command.Execute();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
            command.AttachedArchitecture = this;
            return command.Execute();
        }

        #endregion
    }
}