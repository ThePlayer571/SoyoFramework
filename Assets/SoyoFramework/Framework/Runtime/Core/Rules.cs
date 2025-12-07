using System;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core
{
    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface ICanGetModel : IBelongToArchitecture
    {
    }


    public interface ICanGetSystem : IBelongToArchitecture
    {
    }


    public interface ICanGetService : IBelongToArchitecture
    {
    }


    public interface ICanRegisterEvent : IBelongToArchitecture
    {
    }


    public interface ICanSendService : IBelongToArchitecture
    {
    }

    public interface ICanSendEvent : IBelongToArchitecture
    {
    }


    public interface ICanInit
    {
        bool PreInitialized { get; }
        bool Initialized { get; }
        void PreInit();
        void Init();
        void Deinit();
    }

    #region Extensions

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this IBelongToArchitecture self) where T : class, IModel
        {
            return self.GetArchitecture().GetModel<T>();
        }
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.GetArchitecture().GetSystem<T>();
    }

    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) where T : IEvent =>
            self.GetArchitecture().RegisterEvent<T>(onEvent);

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) where T : IEvent =>
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
    }

    public static class CanSendServiceExtension
    {
        public static void SendService<T>(this ICanSendService self) where T : IService, new() =>
            self.GetArchitecture().SendService<T>(new T());

        public static void SendService<T>(this ICanSendService self, T service) where T : IService =>
            self.GetArchitecture().SendService<T>(service);

        public static TResult SendService<TResult>(this ICanSendService self, IService<TResult> service) =>
            self.GetArchitecture().SendService(service);
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : IEvent, new() =>
            self.GetArchitecture().SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T e) where T : IEvent =>
            self.GetArchitecture().SendEvent<T>(e);
    }

    #endregion
}