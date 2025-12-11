using System;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface ICanRelyOnArchitecture
    {
        IArchitecture RelyingArchitecture { get; }
    }

    public interface ICanAttachToArchitecture : ICanRelyOnArchitecture
    {
        IArchitecture AttachedArchitecture { get; internal set; }
        IArchitecture ICanRelyOnArchitecture.RelyingArchitecture => AttachedArchitecture;
    }

    public interface ICanGetModel : ICanRelyOnArchitecture
    {
    }


    public interface ICanGetSystem : ICanRelyOnArchitecture
    {
    }

    public interface ICanGetService : ICanRelyOnArchitecture
    {
    }

    public interface ICanUnRegisterModel : ICanRelyOnArchitecture
    {
    }

    public interface ICanUnRegisterSystem : ICanRelyOnArchitecture
    {
    }

    public interface ICanUnRegisterService : ICanRelyOnArchitecture
    {
    }


    public interface ICanRegisterEvent : ICanRelyOnArchitecture
    {
    }


    public interface ICanSendEvent : ICanRelyOnArchitecture
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
        public static IProxy<T> GetModel<T>(this ICanGetModel self) where T : class, IModel =>
            self.RelyingArchitecture.GetModel<T>();
    }

    public static class CanGetSystemExtension
    {
        public static IProxy<T> GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.RelyingArchitecture.GetSystem<T>();
    }

    public static class CanGetServiceExtension
    {
        public static IProxy<T> GetService<T>(this ICanGetService self) where T : class, IService =>
            self.RelyingArchitecture.GetService<T>();
    }

    public static class CanUnRegisterModelExtension
    {
        public static void UnRegisterModel<T>(this ICanUnRegisterModel self) where T : class, IModel =>
            self.RelyingArchitecture.UnRegisterModel<T>();
    }

    public static class CanUnRegisterSystemExtension
    {
        public static void UnRegisterSystem<T>(this ICanUnRegisterSystem self) where T : class, ISystem =>
            self.RelyingArchitecture.UnRegisterSystem<T>();
    }
    
    public static class CanUnRegisterServiceExtension
    {
        public static void UnRegisterService<T>(this ICanUnRegisterService self) where T : class, IService =>
            self.RelyingArchitecture.UnRegisterService<T>();
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

    #endregion
}