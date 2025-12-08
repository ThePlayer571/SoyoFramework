using System;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    /// <summary>
    /// 能绑定到Architecture
    /// </summary>
    public interface ICanAttachToArchitecture
    {
        IArchitecture AttachedArchitecture { get; internal set; }
    }

    public interface ICanGetModel : ICanAttachToArchitecture
    {
    }


    public interface ICanGetSystem : ICanAttachToArchitecture
    {
    }

    public interface ICanUnRegisterModel : ICanAttachToArchitecture
    {
    }

    public interface ICanUnRegisterSystem : ICanAttachToArchitecture
    {
    }


    public interface ICanGetService : ICanAttachToArchitecture
    {
    }


    public interface ICanRegisterEvent : ICanAttachToArchitecture
    {
    }


    public interface ICanSendService : ICanAttachToArchitecture
    {
    }

    public interface ICanSendEvent : ICanAttachToArchitecture
    {
    }


    /// <summary>
    /// 约定：初始化方法只能由Architecture调用
    /// </summary>
    public interface ICanInit
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
        public static IProxy<T> GetModel<T>(this ICanAttachToArchitecture self) where T : class, IModel
        {
            return self.AttachedArchitecture.GetModel<T>();
        }
    }

    public static class CanGetSystemExtension
    {
        public static IProxy<T> GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.AttachedArchitecture.GetSystem<T>();
    }

    public static class CanUnRegisterModelExtension
    {
        public static void UnRegisterModel<T>(this ICanUnRegisterModel self) where T : class, IModel =>
            self.AttachedArchitecture.UnRegisterModel<T>();
    }

    public static class CanUnRegisterSystemExtension
    {
        public static void UnRegisterSystem<T>(this ICanUnRegisterSystem self) where T : class, ISystem =>
            self.AttachedArchitecture.UnRegisterSystem<T>();
    }

    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) where T : IEvent =>
            self.AttachedArchitecture.RegisterEvent<T>(onEvent);

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) where T : IEvent =>
            self.AttachedArchitecture.UnRegisterEvent<T>(onEvent);
    }

    public static class CanSendServiceExtension
    {
        public static void SendService<T>(this ICanSendService self) where T : IService, new() =>
            self.AttachedArchitecture.SendService<T>(new T());

        public static void SendService<T>(this ICanSendService self, T service) where T : IService =>
            self.AttachedArchitecture.SendService<T>(service);

        public static TResult SendService<TResult>(this ICanSendService self, IService<TResult> service) =>
            self.AttachedArchitecture.SendService(service);
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : IEvent, new() =>
            self.AttachedArchitecture.SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T e) where T : IEvent =>
            self.AttachedArchitecture.SendEvent<T>(e);
    }

    #endregion
}