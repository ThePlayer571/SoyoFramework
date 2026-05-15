using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils
{
    public partial class EasyEvent<T>
    {
        private readonly List<Action<T>> _callbacks = new List<Action<T>>();

        public IUnRegister Register(Action<T> onEvent)
        {
            if (onEvent != null && !_callbacks.Contains(onEvent))
            {
                _callbacks.Add(onEvent);
            }
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T> onEvent)
        {
            if (onEvent == null) return;
            int index = _callbacks.IndexOf(onEvent);
            if (index >= 0)
            {
                _callbacks[index] = null;
            }
        }

        public void UnRegisterAll()
        {
            _callbacks.Clear();
        }

        public void Trigger(in T arg)
        {
            bool needCleanup = false;

            for (int i = 0; i < _callbacks.Count; i++)
            {
                Action<T> callback = _callbacks[i];
                if (callback == null)
                {
                    needCleanup = true;
                    continue;
                }

                try
                {
                    callback.Invoke(arg);
                }
                catch (Exception e)
                {
                    $"在EasyEvent事件触发中发生异常，已自动处理：\n{e}".LogError();
                }
            }

            if (needCleanup)
            {
                _callbacks.RemoveAll(c => c == null);
            }
        }

        public IUnRegister RegisterWithInvoke(T arg, Action<T> onEvent)
        {
            onEvent?.Invoke(arg);
            return Register(onEvent);
        }
    }

    [Serializable]
    public partial class EasyEvent<T>
    {
#if UNITY_EDITOR
        [SerializeField] private T _arg1;

        /// <summary>
        /// 供 Editor PropertyDrawer 调用的触发方法
        /// </summary>
        internal void EditorTrigger()
        {
            Trigger(_arg1);
        }
#endif
    }
}