using System;
using System.Collections.Generic;
using SoyoFramework.Scripts.ToolKits.Others;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils
{
    public interface IUnRegister
    {
        void UnRegister();
    }

    public class CustomUnRegister : IUnRegister
    {
        private Action _onUnRegister;

        public CustomUnRegister(Action onUnRegister)
        {
            _onUnRegister = onUnRegister;
        }

        public void UnRegister()
        {
            _onUnRegister.Invoke();
            _onUnRegister = null;
        }
    }

    public static class IUnRegisterExtensions
    {
        public static void AddTo(this IUnRegister unRegister, ICollection<IUnRegister> collection)
        {
            collection.Add(unRegister);
        }

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister self, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDestroyListener>().onDestroy.Register(self.UnRegister);
        }

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister self, Component component)
        {
            component.gameObject.GetOrAddComponent<OnDestroyListener>().onDestroy.Register(self.UnRegister);
        }


        // todo 下面两个是临时的
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var comp = self.GetComponent<T>();
            return comp ? comp : self.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component self) where T : Component
        {
            var comp = self.GetComponent<T>();
            return comp ? comp : self.gameObject.AddComponent<T>();
        }
    }
}