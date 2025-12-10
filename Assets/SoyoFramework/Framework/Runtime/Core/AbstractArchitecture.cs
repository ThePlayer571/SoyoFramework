using System;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public abstract class AbstractArchitecture : IArchitecture
    {
        #region 核心

        private readonly ProxyIOCContainer _container = new();

        protected void Init(bool setAsDefault = true)
        {
            if (Inited)
            {
                "Architecture已经初始化".LogError();
                return;
            }

            // 注册初始层级信息
            OnInit();

            // 按次序初始化Model和System
            foreach (var module in _container.GetAll<IModel>())
            {
                module.Get.PreInit();
            }

            foreach (var module in _container.GetAll<ISystem>())
            {
                module.Get.PreInit();
            }

            foreach (var model in _container.GetAll<IModel>())
            {
                model.Get.Init();
            }

            foreach (var system in _container.GetAll<ISystem>())
            {
                system.Get.Init();
            }

            //
            Inited = true;
            if (setAsDefault)
            {
                ArchitectureHelper.DefaultArchitecture = this;
            }
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();


        public bool Inited { get; private set; } = false;

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
            throw new NotImplementedException();
        }

        #endregion


        public void SendService<T>(T service) where T : IService
        {
            throw new NotImplementedException();
        }

        public IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent
        {
            throw new NotImplementedException();
        }

        public void SendEvent<T>() where T : IEvent, new()
        {
            throw new NotImplementedException();
        }

        public void SendEvent<T>(in T e) where T : IEvent
        {
            throw new NotImplementedException();
        }

        public void UnRegisterEvent<T>(Action<T> onEvent) where T : IEvent
        {
            throw new NotImplementedException();
        }
    }
}