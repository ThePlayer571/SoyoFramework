using System;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IArchitecture
    {
        // Architecture生命周期
        void Init(bool setAsDefault = true);
        void Deinit();
        bool Inited { get; }

        // Model层
        void RegisterModel<T>(T model) where T : class, IModel;
        T GetModel<T>() where T : class, IModel;

        // System层
        void RegisterSystem<T>(T system) where T : class, ISystem;
        T GetSystem<T>() where T : class, ISystem;

        // ViewController层
        void RegisterViewController<T>(T viewController) where T : class, IViewController;
        T GetViewController<T>() where T : class, IViewController;

        // Tool支持
        void RegisterTool<T>(T tool) where T : class, ITool;
        T GetTool<T>() where T : class, ITool;

        // Event
        IUnRegister RegisterEvent<T>(Action<T> onEvent) where T : IEvent;
        void SendEvent<T>() where T : IEvent, new();
        void SendEvent<T>(in T e) where T : IEvent;

        // Command
        void SendCommand(ICommand command);
        TResult SendCommand<TResult>(ICommand<TResult> command);
    }

    /// <summary>
    /// 所有能被注册到Architecture的模块的基接口。
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
        ICanGetModel,
        ICanRegisterEvent, ICanSendEvent
    {
    }

    public interface ITool :
        IModule
    {
    }

    public interface IMonoVController :
        ICanRelyOnArchitecture,
        ICanGetModel,
        ICanRegisterEvent, ICanSendCommand
    {
    }

    public interface IViewController :
        IMonoVController, IModule
    {
    }

    public interface ICommand :
        ICanAttachToArchitecture,
        ICanGetModel,
        ICanSendEvent, ICanSendCommand
    {
        void Execute();
    }

    public interface ICommand<out TResult> :
        ICanAttachToArchitecture,
        ICanGetModel,
        ICanSendEvent, ICanSendCommand
    {
        TResult Execute();
    }
}