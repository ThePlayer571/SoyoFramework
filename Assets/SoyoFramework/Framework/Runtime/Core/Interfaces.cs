using System;
using System.Diagnostics.CodeAnalysis;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IArchitecture
    {
        // Architecture信息
        bool Inited { get; }

        // Module
        //  UnRegister会触发Deinit
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

        // Service

        // Event
        IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent;
        void SendEvent<T>() where T : IEvent, new();
        void SendEvent<T>(in T e) where T : IEvent;
        void UnRegisterEvent<T>(Action<T> onEvent) where T : IEvent;
    }

    public interface IModule :
        ICanAttachToArchitecture, ICanInit
    {
    }

    public interface IModel :
        IModule,
        ICanSendEvent
    {
    }

    public interface ISystem :
        IModule,
        ICanGetModel, ICanUnRegisterModel, ICanUnRegisterSystem,
        ICanRegisterEvent, ICanSendEvent
    {
    }


    public interface IService :
        IModule,
        ICanGetModel, ICanGetService
    {
    }

    public interface IController :
        IModule,
        ICanGetModel, ICanGetService,
        ICanRegisterEvent, ICanSendEvent
    {
    }


    // public interface IController : IBelongToArchitecture, ICanSetArchitecture, ICanSendService, ICanGetModel,
    //     ICanGetSystem, ICanRegisterEvent, ICanGetService
    // {
    // }
    //
    // public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanGetService,
    //     ICanSendEvent, ICanRegisterEvent, ICanInit
    // {
    // }
    //
    // public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanInit
    // {
    // }
    //
    // public interface IService : IBelongToArchitecture, ICanGetSystem, ICanGetModel, ICanGetService, ICanSendEvent,
    //     ICanSendService, ICanSetArchitecture
    // {
    //     void Execute();
    // }
    //
    // public interface IService<out TResult> : IBelongToArchitecture, ICanGetSystem, ICanGetModel, ICanGetService,
    //     ICanSendEvent, ICanSendService
    //     , ICanSetArchitecture
    // {
    //     TResult Execute();
    // }
}