using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Utils
{
    public partial class EasyEvent<T1, T2>
    {
        private readonly List<Action<T1, T2>> _callbacks = new();

        public IUnRegister Register(Action<T1, T2> onEvent)
        {
            _callbacks.Add(onEvent);
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T1, T2> onEvent)
        {
            _callbacks.Remove(onEvent);
        }

        public void UnRegisterAll()
        {
            _callbacks.Clear();
        }

        public void Trigger(in T1 arg1, in T2 arg2)
        {
            if (_callbacks.Count == 0) return;

            var snapshot = ListPool<Action<T1, T2>>.Rent();
            snapshot.AddRange(_callbacks);

            foreach (var callback in snapshot)
            {
                try
                {
                    callback.Invoke(arg1, arg2);
                }
                catch (Exception e)
                {
                    $"在EasyEvent事件触发中发生异常，已自动处理：\n{e}".LogError();
                }
            }

            ListPool<Action<T1, T2>>.Return(snapshot);
        }

        public IUnRegister RegisterWithInvoke(T1 arg1, T2 arg2, Action<T1, T2> onEvent)
        {
            onEvent.Invoke(arg1, arg2);
            return Register(onEvent);
        }
    }

    [Serializable]
    public partial class EasyEvent<T1, T2>
    {
#if UNITY_EDITOR
        [SerializeField] private T1 _arg1 = default!;
        [SerializeField] private T2 _arg2 = default!;

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