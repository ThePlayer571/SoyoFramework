using System;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IArchitecture
    {
        // Architecture生命周期
        /// <summary>
        /// Architecture是否已完成初始化。只有初始化后的Architecture才能正常使用。通常会自动初始化，如果出现问题，尝试手动调用Architecture.Init()
        /// </summary>
        bool Inited { get; }

        // Module（所有层级的超集）
        /// <summary>
        /// 注册一个Module。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="module"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterModule<T>(T module) where T : class, IModule;

        /// <summary>
        /// 获取一个Module。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModule<T>() where T : class, IModule;

        // Model层
        /// <summary>
        /// 注册一个Model。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterModel<T>(T model) where T : class, IModel;

        /// <summary>
        /// 获取一个Model。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModel<T>() where T : class, IModel;

        // System层
        /// <summary>
        /// 注册一个System。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="system"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterSystem<T>(T system) where T : class, ISystem;

        /// <summary>
        /// 获取一个System。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSystem<T>() where T : class, ISystem;

        // ViewController层
        /// <summary>
        /// 注册一个ViewController。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="viewController"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterVController<T>(T viewController) where T : class, IVController;

        /// <summary>
        /// 获取一个ViewController。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetVController<T>() where T : class, IVController;

        // Sublayer支持
        /// <summary>
        /// 注册一个Sublayer。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="sublayer"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterSublayer<T>(T sublayer) where T : class, ISublayer;

        /// <summary>
        /// 获取一个Sublayer。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSublayer<T>() where T : class, ISublayer;

        // Event
        /// <summary>
        /// 按类型注册事件，会在事件发送时调用onEvent。返回一个IUnRegister，调用它可以取消注册
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IUnRegister RegisterEvent<T>(Action<T> onEvent);

        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。这是无需传入实例的语法糖，T必须包含无参构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void SendEvent<T>() where T : new();

        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。
        /// </summary>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        void SendEvent<T>(in T e);

        // Command
        /// <summary>
        /// 初始化一个Command。初始化后，可以直接调用CanExecute获取CanExecuteResult。想调用Command，必须通过SendCommand
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T InitCommand<T>(T command) where T : ICommand;

        /// <summary>
        /// 发送一个Command
        /// </summary>
        /// <param name="command"></param>
        void SendCommand(ICommand command);

        /// <summary>
        /// 发送一个Command，并获取返回值
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        void SendCommand<TResult>(ICommand<TResult> command, out TResult result);

        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        CanExecuteResult TrySendCommand(ICommand command);

        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult和返回值
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
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

    public interface ISublayer :
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

    public interface ICommand<out TResult> : ICommand,
        ICanAttachToArchitecture, ICommandRule
    {
        /// <summary>
        /// 执行Command的逻辑，推荐只能通过Architecture来调用
        /// </summary>
        /// <param name="ignoreCanExecuteCheck">执行时不自动调用CanExecute检查，通常为了性能而开启</param>
        protected internal new TResult Execute(bool ignoreCanExecuteCheck = false);
    }
}