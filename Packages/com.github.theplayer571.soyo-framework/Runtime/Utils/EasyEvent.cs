using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework.Utils
{
    public partial class EasyEvent
    {
        private readonly List<Action> _callbacks = new();

        public IUnRegister Register(Action onEvent)
        {
            _callbacks.Add(onEvent);
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action onEvent)
        {
            _callbacks.Remove(onEvent);
        }

        public void UnRegisterAll()
        {
            _callbacks.Clear();
        }

        public void Trigger()
        {
            if (_callbacks.Count == 0) return;

            var snapshot = ListPool<Action>.Rent();
            snapshot.AddRange(_callbacks);

            foreach (var callback in snapshot)
            {
                try
                {
                    callback.Invoke();
                }
                catch (Exception e)
                {
                    $"在EasyEvent事件触发中发生异常，已自动处理：\n{e}".LogError();
                }
            }

            ListPool<Action>.Return(snapshot);
        }

        public IUnRegister RegisterWithInvoke(Action onEvent)
        {
            onEvent.Invoke();
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