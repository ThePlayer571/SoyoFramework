using System;
using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface ICanRelyOnArchitecture
    {
        IArchitecture RelyingArchitecture { get; }
    }

    public interface ICanAttachToArchitecture : ICanRelyOnArchitecture
    {
        IArchitecture AttachedArchitecture { get; set; }
        IArchitecture ICanRelyOnArchitecture.RelyingArchitecture => AttachedArchitecture;
    }

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

        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
            self.RelyingArchitecture.SendCommand(command);
    }

    #endregion
}