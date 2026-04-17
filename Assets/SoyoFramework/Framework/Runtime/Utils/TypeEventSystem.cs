using System;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;

namespace SoyoFramework.Framework.Runtime.Utils
{
    public class TypeEventSystem
    {
        private readonly SimpleIOCContainer _events = new();

        public void Call<T>() where T : new()
        {
            Call(new T());
        }

        public void Call<T>(in T e)
        {
            var easyEvent = _events.Get<EasyEvent<T>>();
            easyEvent?.Trigger(in e);
        }

        public IUnRegister Register<T>(Action<T> callback)
        {
            var easyEvent = _events.Get<EasyEvent<T>>();
            if (easyEvent == null)
            {
                easyEvent = new EasyEvent<T>();
                _events.Register<EasyEvent<T>>(easyEvent);
            }

            return easyEvent.Register(callback);
        }

        public void UnRegister<T>(Action<T> callback)
        {
            var easyEvent = _events.Get<EasyEvent<T>>();
            easyEvent?.UnRegister(callback);
        }

        public void Clear() => _events.Clear();
    }
}