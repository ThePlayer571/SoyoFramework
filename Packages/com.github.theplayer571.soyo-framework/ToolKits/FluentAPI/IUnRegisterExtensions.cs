using System.Collections.Generic;
using SoyoFramework.ToolKits.UnityListener;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;
using UnityEngine.Events;

namespace SoyoFramework.ToolKits.FluentAPI
{
    public static class IUnRegisterExtensions
    {
        public static void AddTo(this IUnRegister unRegister, ICollection<IUnRegister> collection)
        {
            collection.Add(unRegister);
        }

        public static void AddTo(this IUnRegister unRegister, UnRegisterGroup unRegisterGroup)
        {
            unRegisterGroup.Add(unRegister);
        }

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister self, GameObject? gameObject)
        {
            if (gameObject == null) return;
            gameObject.GetOrAddComponent<OnDestroyListener>().onDestroy.Register(self.UnRegister);
        }

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister self, Component? component)
        {
            if (component == null) return;
            component.gameObject.GetOrAddComponent<OnDestroyListener>().onDestroy.Register(self.UnRegister);
        }

        // ====================== UnityEvent ======================

        public static IUnRegister Register(this UnityEvent unityEvent, UnityAction call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }

        public static IUnRegister Register<T0>(this UnityEvent<T0> unityEvent, UnityAction<T0> call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }

        public static IUnRegister Register<T0, T1>(this UnityEvent<T0, T1> unityEvent, UnityAction<T0, T1> call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }

        public static IUnRegister Register<T0, T1, T2>(this UnityEvent<T0, T1, T2> unityEvent,
            UnityAction<T0, T1, T2> call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }

        public static IUnRegister Register<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent,
            UnityAction<T0, T1, T2, T3> call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }
    }
}