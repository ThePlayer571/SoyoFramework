using System;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.CoreUtils
{
    public partial class EasyEvent<T>
    {
        private Action<T> _onEvent;

        public IUnRegister Register(Action<T> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public IUnRegister RegisterWithInvoke(T arg, Action<T> onEvent)
        {
            onEvent?.Invoke(arg);
            return Register(onEvent);
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

    [Serializable]
    public partial class EasyEvent<T>
    {
#if UNITY_EDITOR
        [SerializeField] private T _serializedValue;

        /// <summary>
        /// 供 Editor PropertyDrawer 调用的触发方法
        /// </summary>
        internal void EditorTrigger()
        {
            Trigger(_serializedValue);
        }
#endif
    }
}