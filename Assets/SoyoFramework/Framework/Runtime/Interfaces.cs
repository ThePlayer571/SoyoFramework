using System;
using SoyoFramework.Framework.CoreKits;

namespace SoyoFramework.Framework.Runtime
{
    public interface IArchitecture
    {
        void RegisterSystem<T>(T system) where T : ISystem;
        void RegisterModel<T>(T model) where T : IModel;


        T GetSystem<T>() where T : class, ISystem;

        T GetModel<T>() where T : class, IModel;
        void SendService<T>(T service) where T : IService;

        TResult SendService<TResult>(IService<TResult> service);

        void SendEvent<T>() where T : IEvent, new();
        void SendEvent<T>(in T e) where T : IEvent;

        IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent;

        void UnRegisterEvent<T>(Action<T> onEvent) where T : IEvent;

        //
        void Init(bool setAsDefault = true);
        void Deinit();
    }

    public interface IController : ICanSendService, ICanGetModel, ICanGetSystem, ICanRegisterEvent, ICanGetService
    {
    }

    public interface ISystem : ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanGetService, ICanSendEvent,
        ICanRegisterEvent, ICanInit
    {
    }

    public interface IModel : ICanSetArchitecture, ICanInit
    {
    }

    public interface IService : ICanGetSystem, ICanGetModel, ICanGetService, ICanSendEvent, ICanSendService,
        ICanSetArchitecture
    {
        void Execute();
    }

    public interface IService<out TResult> : ICanGetSystem, ICanGetModel, ICanGetService, ICanSendEvent, ICanSendService
        , ICanSetArchitecture
    {
        TResult Execute();
    }
}