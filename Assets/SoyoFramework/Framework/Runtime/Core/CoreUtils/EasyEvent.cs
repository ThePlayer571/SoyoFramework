using System;

namespace SoyoFramework.Framework.Runtime.Core.CoreUtils
{
    public class EasyEvent
    {
        private Action _onEvent;

        public IUnRegister Register(Action onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Trigger()
        {
            _onEvent?.Invoke();
        }
    }

    public class EasyEvent<T>
    {
        private Action<T> _onEvent;

        public IUnRegister Register(Action<T> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T> onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Trigger(in T arg)
        {
            _onEvent?.Invoke(arg);
        }
    }

    public class EasyEvent<T1, T2>
    {
        private Action<T1, T2> _onEvent;

        public IUnRegister Register(Action<T1, T2> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T1, T2> onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Trigger(in T1 arg1, in T2 arg2)
        {
            _onEvent?.Invoke(arg1, arg2);
        }
    }

    public class EasyEvent<T1, T2, T3>
    {
        private Action<T1, T2, T3> _onEvent;

        public IUnRegister Register(Action<T1, T2, T3> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T1, T2, T3> onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Trigger(in T1 arg1, in T2 arg2, in T3 arg3)
        {
            _onEvent?.Invoke(arg1, arg2, arg3);
        }
    }
}