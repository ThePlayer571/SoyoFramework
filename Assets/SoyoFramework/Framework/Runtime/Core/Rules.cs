using System;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;

namespace SoyoFramework.Framework.Runtime.Core
{
    #region 接口：框架依赖

    public interface ICanRelyOnArchitecture
    {
        IArchitecture RelyingArchitecture { get; }
    }

    public interface ICanAttachToArchitecture : ICanRelyOnArchitecture
    {
        IArchitecture AttachedArchitecture { get; set; }
        IArchitecture ICanRelyOnArchitecture.RelyingArchitecture => AttachedArchitecture;
    }

    /// <summary>
    /// 约定：初始化方法只能由Architecture调用
    /// </summary>
    public interface ICanInitByArchitecture : ICanAttachToArchitecture
    {
        bool PreInitialized { get; }
        bool Initialized { get; }

        /// <summary>
        /// 类似Awake：用于内部初始化逻辑，禁止获取其他Module
        /// </summary>
        internal void PreInit();

        /// <summary>
        /// 类似Start：可以获取其他Module
        /// </summary>
        internal void Init();

        internal void Deinit();
    }

    #endregion

    #region 接口：基础规则

    public interface ICanRegisterEvent : ICanRelyOnArchitecture
    {
    }

    public interface ICanSendEvent : ICanRelyOnArchitecture
    {
    }

    public interface ICanSendCommand : ICanRelyOnArchitecture
    {
    }

    public interface ICanRegisterDomainRoot : ICanRelyOnArchitecture
    {
    }

    public interface ICanGetDomainRoot : ICanRelyOnArchitecture
    {
    }

    public interface ICanUnregisterDomainRoot : ICanRelyOnArchitecture
    {
    }

    public interface ICanGetDomainService : ICanRelyOnArchitecture
    {
    }

    [SuperLayer("获取任意层级")]
    public interface ICanGet<T> : ICanRelyOnArchitecture
        where T : class, IModule
    {
    }

    #endregion

    #region 接口：层级规则

    public interface IDomainRule :
        ICanRegisterEvent, ICanSendEvent
    {
    }


    public interface IViewControllerRule :
        ICanRegisterEvent, ICanSendCommand
    {
    }

    public interface ICommandRule :
        ICanSendEvent, ICanSendCommand,
        ICanGetDomainRoot, ICanRegisterDomainRoot, ICanUnregisterDomainRoot, 
        ICanGetDomainService
    {
    }

    public interface IDomainServiceRule :
        ICanSendEvent
    {
        
    }

    #endregion

    #region Extensions

    public static class CanRegisterEventExtension
    {
        /// <summary>
        /// 按类型注册事件，会在事件发送时调用onEvent。返回一个IUnRegister，调用它可以取消注册
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
            => self.RelyingArchitecture.RegisterEvent<T>(onEvent);
    }

    public static class CanSendEventExtension
    {
        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。这是无需传入实例的语法糖，T必须包含无参构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SendEvent<T>(this ICanSendEvent self) where T : new()
            => self.RelyingArchitecture.SendEvent<T>();

        /// <summary>
        /// 按类型发送事件，会调用所有注册了该类型事件的回调。
        /// </summary>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        public static void SendEvent<T>(this ICanSendEvent self, T e)
            => self.RelyingArchitecture.SendEvent<T>(e);
    }

    public static class CanSendCommandExtension
    {
        /// <summary>
        /// 初始化一个Command。初始化后，可以直接调用CanExecute获取CanExecuteResult。想调用Command，必须通过SendCommand
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T InitCommand<T>(this ICanSendCommand self, T command) where T : ICommand
            => self.RelyingArchitecture.InitCommand(command);

        /// <summary>
        /// 发送一个Command
        /// </summary>
        /// <param name="command"></param>
        public static void SendCommand(this ICanSendCommand self, ICommand command)
            => self.RelyingArchitecture.SendCommand(command);

        /// <summary>
        /// 发送一个Command，并获取返回值
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        public static void SendCommand<TResult>
            (this ICanSendCommand self, ICommand<TResult> command, out TResult result)
            => self.RelyingArchitecture.SendCommand(command, out result);

        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CanExecuteResult TrySendCommand(this ICanSendCommand self, ICommand command)
            => self.RelyingArchitecture.TrySendCommand(command);

        /// <summary>
        /// 尝试发送一个Command，并返回CanExecuteResult和返回值
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static CanExecuteResult TrySendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command,
            out TResult result)
            => self.RelyingArchitecture.TrySendCommand(command, out result);
    }

    public static class CanRegisterDomainRootExtension
    {
        /// <summary>
        /// 注册一个DomainRoot。会以T为key存储在IOCContainer里
        /// </summary>
        /// <param name="domainRoot"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterDomainRoot<T>(this ICanRegisterDomainRoot self, T domainRoot)
            where T : class, IDomainRoot
        {
            self.RelyingArchitecture.RegisterDomainRoot(domainRoot);
        }
    }

    public static class CanGetDomainRootExtension
    {
        /// <summary>
        /// 获取一个DomainRoot。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDomainRoot<T>(this ICanGetDomainRoot self)
            where T : class, IDomainRoot
        {
            return self.RelyingArchitecture.GetDomainRoot<T>();
        }
    }

    public static class CanUnregisterDomainRootExtension
    {
        /// <summary>
        /// 卸载一个DomainRoot。会以T为key从IOCContainer里移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UnregisterDomainRoot<T>(this ICanUnregisterDomainRoot self)
            where T : class, IDomainRoot
        {
            self.RelyingArchitecture.UnregisterDomainRoot<T>();
        }
    }
    
    public static class CanGetDomainServiceExtension
    {
        /// <summary>
        /// 获取一个DomainService。会以T为key从IOCContainer里取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDomainService<T>(this ICanGetDomainService self)
            where T : class, IDomainService
        {
            return self.RelyingArchitecture.GetModule<T>();
        }
    }

    public static class CanGetExtension
    {
        /// <summary>
        /// 无视规则强行获取Module
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(this ICanGet<T> self) where T : class, IModule
        {
            return self.RelyingArchitecture.GetModule<T>();
        }
    }

    #endregion
}