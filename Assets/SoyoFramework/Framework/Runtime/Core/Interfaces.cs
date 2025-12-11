using System;
using System.Diagnostics.CodeAnalysis;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IArchitecture
    {
        // Architecture生命周期
        void Init(bool setAsDefault = true);
        void Deinit();
        bool Inited { get; }

        // Module
        // UnRegister 会触发Deinit
        void RegisterModel<T>(T model) where T : class, IModel;
        void RegisterSystem<T>(T system) where T : class, ISystem;
        void RegisterService<T>(T service) where T : class, IService;
        void UnRegisterModel<T>() where T : class, IModel;
        void UnRegisterSystem<T>() where T : class, ISystem;
        void UnRegisterService<T>() where T : class, IService;

        [return: NotNull]
        IProxy<T> GetSystem<T>() where T : class, ISystem;

        [return: NotNull]
        IProxy<T> GetModel<T>() where T : class, IModel;

        [return: NotNull]
        IProxy<T> GetService<T>() where T : class, IService;

        // Event
        IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent;
        void SendEvent<T>() where T : IEvent, new();
        void SendEvent<T>(in T e) where T : IEvent;
    }

    /// <summary>
    /// 所有能被注册到Architecture的模块的基接口。
    /// 实现了ICanInitByArchitecture，生命周期为 未初始化 -> PreInited -> Inited -> Deinit -> 销毁
    /// </summary>
    public interface IModule :
        ICanAttachToArchitecture, ICanInitByArchitecture
    {
    }

    public interface IModel :
        IModule,
        ICanSendEvent
    {
    }

    public interface ISystem :
        IModule,
        ICanGetModel, ICanGetService,
        ICanUnRegisterModel, ICanUnRegisterSystem,
        ICanRegisterEvent, ICanSendEvent
    {
    }


    public interface IService :
        IModule,
        ICanGetModel, ICanGetService
    {
    }

    public interface IViewController :
        ICanRelyOnArchitecture,
        ICanGetModel, ICanGetService,
        ICanRegisterEvent, ICanSendEvent
    {
    }
}