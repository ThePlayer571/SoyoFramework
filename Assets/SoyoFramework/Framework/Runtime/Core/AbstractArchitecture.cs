using System;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public abstract class AbstractArchitecture : IArchitecture
    {
        #region 可用字段

        public bool Inited { get; private set; } = false;
        private readonly ProxyIOCContainer _container = new();
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
                module.Get.PreInit();
            }

            foreach (var module in _container.GetAll<IModule>())
            {
                module.Get.Init();
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
                module.Get.Deinit();
                module.SetInstance(null);
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

        public void RegisterService<T>(T service) where T : class, IService
        {
            service.AttachedArchitecture = this;
            _container.Register<T>(service);

            // 如果是在Architecture初始化后注册的Module，直接初始化
            if (Inited)
            {
                service.PreInit();
                service.Init();
            }
        }

        public void UnRegisterModel<T>() where T : class, IModel
        {
            var proxy = _container.Get<T>();
            proxy.Get.Deinit();
            proxy.SetInstance(null);
        }

        public void UnRegisterSystem<T>() where T : class, ISystem
        {
            var proxy = _container.Get<T>();
            proxy.Get.Deinit();
            proxy.SetInstance(null);
        }

        public void UnRegisterService<T>() where T : class, IService
        {
            var proxy = _container.Get<T>();
            proxy.Get.Deinit();
            proxy.SetInstance(null);
        }

        public IProxy<T> GetSystem<T>() where T : class, ISystem
        {
            return _container.Get<T>();
        }

        public IProxy<T> GetModel<T>() where T : class, IModel
        {
            return _container.Get<T>();
        }

        public IProxy<T> GetService<T>() where T : class, IService
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

        #endregion
    }
}