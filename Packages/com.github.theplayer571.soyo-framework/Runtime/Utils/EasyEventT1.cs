using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Utils
{
    public partial class EasyEvent<T>
    {
        private readonly List<Action<T>> _callbacks = new();

        public IUnRegister Register(Action<T> onEvent)
        {
            _callbacks.Add(onEvent);
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T> onEvent)
        {
            _callbacks.Remove(onEvent);
        }

        public void UnRegisterAll()
        {
            _callbacks.Clear();
        }

        public void Trigger(in T arg)
        {
            if (_callbacks.Count == 0) return;

            var snapshot = ListPool<Action<T>>.Rent();
            snapshot.AddRange(_callbacks);

            foreach (var callback in snapshot)
            {
                try
                {
                    callback.Invoke(arg);
                }
                catch (Exception e)
                {
                    $"在EasyEvent事件触发中发生异常，已自动处理：\n{e}".LogError();
                }
            }

            ListPool<Action<T>>.Return(snapshot);
        }

        public IUnRegister RegisterWithInvoke(T arg, Action<T> onEvent)
        {
            onEvent.Invoke(arg);
            return Register(onEvent);
        }
    }

    [Serializable]
    public partial class EasyEvent<T>
    {
#if UNITY_EDITOR
        [SerializeField] private T _arg1 = default!;

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