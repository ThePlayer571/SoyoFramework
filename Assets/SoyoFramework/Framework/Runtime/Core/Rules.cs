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

    public interface ICanGetModel : ICanRelyOnArchitecture
    {
    }


    public interface ICanRegisterEvent : ICanRelyOnArchitecture
    {
    }


    public interface ICanSendEvent : ICanRelyOnArchitecture
    {
    }

    public interface ICanSendCommand : ICanRelyOnArchitecture
    {
    }

    [SuperLayer("获取任意层级")]
    public interface ICanGet<T> : ICanRelyOnArchitecture
        where T : class, IModule
    {
    }

    #endregion

    #region 接口：层级规则

    public interface IModelRule :
        ICanSendEvent
    {
    }

    public interface ISystemRule :
        ICanGetModel,
        ICanRegisterEvent, ICanSendEvent
    {
    }

    public interface IViewControllerRule :
        ICanGetModel,
        ICanRegisterEvent, ICanSendCommand
    {
    }

    public interface ICommandRule :
        ICanGetModel,
        ICanSendEvent, ICanSendCommand
    {
    }

    #endregion
    
    #region Extensions

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel =>
            self.RelyingArchitecture.GetModel<T>();
    }

    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) where T : IEvent =>
            self.RelyingArchitecture.RegisterEvent<T>(onEvent);
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : IEvent, new() =>
            self.RelyingArchitecture.SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T e) where T : IEvent =>
            self.RelyingArchitecture.SendEvent<T>(e);
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand(this ICanSendCommand self, ICommand command) =>
            self.RelyingArchitecture.SendCommand(command);

        public static void SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command,
            out TResult result) =>
            self.RelyingArchitecture.SendCommand(command, out result);

        public static CanExecuteResult TrySendCommand(this ICanSendCommand self, ICommand command)
            => self.RelyingArchitecture.TrySendCommand(command);

        public static CanExecuteResult TrySendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command,
            out TResult result)
            => self.RelyingArchitecture.TrySendCommand(command, out result);
    }

    public static class CanGetExtension
    {
        public static T Get<T>(this ICanGet<T> self) where T : class, IModule
        {
            return self.RelyingArchitecture.GetModule<T>();
        }
    }

    #endregion
}