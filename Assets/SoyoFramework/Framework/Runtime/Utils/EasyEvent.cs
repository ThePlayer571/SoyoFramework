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
        [SerializeField, UsedImplicitly] private bool _dummyValue = false; // todo （看看这个是否真的需要）占位，确保Unity序列化正常
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