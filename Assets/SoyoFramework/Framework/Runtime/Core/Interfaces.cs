using System;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IArchitecture
    {
        // Architecture信息
        bool Inited { get; }

        // 基础层级
        void RegisterModel<T>(T model) where T : class, IModel;
        void RegisterSystem<T>(T system) where T : class, ISystem;

        // System
        IProxy<T> GetSystem<T>() where T : class, ISystem;

        // Model
        IProxy<T> GetModel<T>() where T : class, IModel;

        // Service
        void SendService<T>(T service) where T : IService;
        TResult SendService<TResult>(IService<TResult> service);

        // Event
        IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent;
        void SendEvent<T>() where T : IEvent, new();
        void SendEvent<T>(in T e) where T : IEvent;
        void UnRegisterEvent<T>(Action<T> onEvent) where T : IEvent;
    }
    
    public interface ISystem : ICanAttachToArchitecture, ICanInit, ICanGetModel
    {
    }

    public interface IModel : ICanAttachToArchitecture, ICanInit
    {
    }

    public interface IController
    {
    }


    public interface IService
    {
        void Execute();
    }

    public interface IService<out TResult>
    {
        TResult Execute();
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