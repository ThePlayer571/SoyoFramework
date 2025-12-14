using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.UnityListener;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.FluentAPI
{
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
    }
}