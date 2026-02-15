using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using SoyoFramework.ToolKits.Runtime.UnityListener;
using UnityEngine;
using UnityEngine.Events;

namespace SoyoFramework.ToolKits.Runtime.FluentAPI
{
    public static class IUnRegisterExtensions
    {
        public static void AddTo(this IUnRegister unRegister, ICollection<IUnRegister> collection)
        {
            collection.Add(unRegister);
        }

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister self, GameObject gameObject)
        {
            gameObject?.GetOrAddComponent<OnDestroyListener>().onDestroy.Register(self.UnRegister);
        }

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister self, Component component)
        {
            component?.gameObject.GetOrAddComponent<OnDestroyListener>().onDestroy.Register(self.UnRegister);
        }
        
        // ====================== Action ======================
        public static IUnRegister Register(this Action handler, Action<Action> add, Action<Action> remove)
        {
            add(handler);
            return new CustomUnRegister(() => remove(handler));
        }

        public static IUnRegister Register<T>(this Action<T> handler, Action<Action<T>> add, Action<Action<T>> remove)
        {
            add(handler);
            return new CustomUnRegister(() => remove(handler));
        }

        public static IUnRegister Register<T1, T2>(this Action<T1, T2> handler, Action<Action<T1, T2>> add, Action<Action<T1, T2>> remove)
        {
            add(handler);
            return new CustomUnRegister(() => remove(handler));
        }

        public static IUnRegister Register<T1, T2, T3>(this Action<T1, T2, T3> handler, Action<Action<T1, T2, T3>> add, Action<Action<T1, T2, T3>> remove)
        {
            add(handler);
            return new CustomUnRegister(() => remove(handler));
        }

        public static IUnRegister Register<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> handler, Action<Action<T1, T2, T3, T4>> add, Action<Action<T1, T2, T3, T4>> remove)
        {
            add(handler);
            return new CustomUnRegister(() => remove(handler));
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
        public static IUnRegister Register<T0, T1, T2>(this UnityEvent<T0, T1, T2> unityEvent, UnityAction<T0, T1, T2> call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }
        public static IUnRegister Register<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent, UnityAction<T0, T1, T2, T3> call)
        {
            unityEvent.AddListener(call);
            return new CustomUnRegister(() => unityEvent.RemoveListener(call));
        }
    }
}