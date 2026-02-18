using System;
using JetBrains.Annotations;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils
{
    public partial class EasyEvent
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

        public IUnRegister RegisterWithInvoke(Action onEvent)
        {
            onEvent?.Invoke();
            return Register(onEvent);
        }
    }

    [Serializable]
    public partial class EasyEvent
    {
#if UNITY_EDITOR
        /// <summary>
        /// 供 Editor PropertyDrawer 调用的触发方法
        /// </summary>
        internal void EditorTrigger()
        {
            Trigger();
        }
#endif
    }
}