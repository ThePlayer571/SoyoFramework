using System;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IArchitecture
    {
        // Architecture生命周期
        bool Inited { get; }

        // Module（所有层级的超集）
        void RegisterModule<T>(T module) where T : class, IModule;
        T GetModule<T>() where T : class, IModule;

        // Model层
        void RegisterModel<T>(T model) where T : class, IModel;
        T GetModel<T>() where T : class, IModel;

        // System层
        void RegisterSystem<T>(T system) where T : class, ISystem;
        T GetSystem<T>() where T : class, ISystem;

        // ViewController层
        void RegisterVController<T>(T viewController) where T : class, IVController;
        T GetVController<T>() where T : class, IVController;

        // Tool支持
        void RegisterTool<T>(T tool) where T : class, ITool;
        T GetTool<T>() where T : class, ITool;

        // Event
        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void SendEvent<T>() where T : new();
        void SendEvent<T>(in T e);

        // Command
        void SendCommand(ICommand command);
        void SendCommand<TResult>(ICommand<TResult> command, out TResult result);
        CanExecuteResult TrySendCommand(ICommand command);
        CanExecuteResult TrySendCommand<TResult>(ICommand<TResult> command, out TResult result);
    }

    /// <summary>
    /// 基接口：能被注册到Architecture中
    /// </summary>
    public interface IModule :
        ICanAttachToArchitecture, ICanInitByArchitecture
    {
    }


    public interface IModel :
        IModule, IModelRule
    {
    }

    public interface ISystem :
        IModule, ISystemRule
    {
    }

    public interface ITool :
        IModule
    {
    }

    public interface IMonoVController :
        IViewControllerRule
    {
    }

    public interface IVController :
        IModule, IViewControllerRule
    {
    }


    public interface ICommand :
        ICanAttachToArchitecture, ICommandRule
    {
        /// <summary>
        /// 执行Command的逻辑，推荐只能通过Architecture来调用
        /// </summary>
        /// <param name="ignoreCanExecuteCheck">执行时不自动调用CanExecute检查，通常为了性能而开启</param>
        protected internal void Execute(bool ignoreCanExecuteCheck = false);

        CanExecuteResult CanExecute();
    }

    public interface ICommand<out TResult> :
        ICanAttachToArchitecture, ICommandRule
    {
        /// <summary>
        /// 执行Command的逻辑，推荐只能通过Architecture来调用
        /// </summary>
        /// <param name="ignoreCanExecuteCheck">执行时不自动调用CanExecute检查，通常为了性能而开启</param>
        protected internal TResult Execute(bool ignoreCanExecuteCheck = false);

        CanExecuteResult CanExecute();
    }
}