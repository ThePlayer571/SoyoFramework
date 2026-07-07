using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Utils
{
    public partial class EasyEvent<T1, T2, T3>
    {
        private readonly List<Action<T1, T2, T3>> _callbacks = new List<Action<T1, T2, T3>>();

        public IUnRegister Register(Action<T1, T2, T3> onEvent)
        {
            if (onEvent != null && !_callbacks.Contains(onEvent))
            {
                _callbacks.Add(onEvent);
            }
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T1, T2, T3> onEvent)
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

        public void Trigger(in T1 arg1, in T2 arg2, in T3 arg3)
        {
            bool needCleanup = false;

            for (int i = 0; i < _callbacks.Count; i++)
            {
                Action<T1, T2, T3> callback = _callbacks[i];
                if (callback == null)
                {
                    needCleanup = true;
                    continue;
                }

                try
                {
                    callback.Invoke(arg1, arg2, arg3);
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

        public IUnRegister RegisterWithInvoke(T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> onEvent)
        {
            onEvent?.Invoke(arg1, arg2, arg3);
            return Register(onEvent);
        }
    }

    [Serializable]
    public partial class EasyEvent<T1, T2, T3>
    {
#if UNITY_EDITOR
        [SerializeField] private T1 _arg1;
        [SerializeField] private T2 _arg2;
        [SerializeField] private T3 _arg3;

        /// <summary>
        /// 供 Editor PropertyDrawer 调用的触发方法
        /// </summary>
        internal void EditorTrigger()
        {
            Trigger(_arg1, _arg2, _arg3);
        }
#endif
    }
}