using System;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils
{
    public partial class EasyEvent<T1, T2>
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
        public void UnRegisterAll()
        {
            _onEvent = null;
        }

        public void Trigger(in T1 arg1, in T2 arg2)
        {
            _onEvent?.Invoke(arg1, arg2);
        }

        public IUnRegister RegisterWithInvoke(T1 arg1, T2 arg2, Action<T1, T2> onEvent)
        {
            onEvent?.Invoke(arg1, arg2);
            return Register(onEvent);
        }
    }

    [Serializable]
    public partial class EasyEvent<T1, T2>
    {
#if UNITY_EDITOR
        [SerializeField] private T1 _arg1;
        [SerializeField] private T2 _arg2;

        /// <summary>
        /// 供 Editor PropertyDrawer 调用的触发方法
        /// </summary>
        internal void EditorTrigger()
        {
            Trigger(_arg1, _arg2);
        }
#endif
    }
}