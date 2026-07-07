using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework.Utils
{
    public partial class EasyEvent
    {
        private readonly List<Action> _callbacks = new List<Action>();

        public IUnRegister Register(Action onEvent)
        {
            if (onEvent != null && !_callbacks.Contains(onEvent))
            {
                _callbacks.Add(onEvent);
            }

            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action onEvent)
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

        public void Trigger()
        {
            bool needCleanup = false;

            for (int i = 0; i < _callbacks.Count; i++)
            {
                Action callback = _callbacks[i];
                if (callback == null)
                {
                    needCleanup = true;
                    continue;
                }

                try
                {
                    callback.Invoke();
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