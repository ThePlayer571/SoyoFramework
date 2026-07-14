using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework.Utils
{
    public partial class EasyEvent
    {
        private readonly List<Action?> _callbacks = new();
        private readonly List<Action> _pendingAdds = new();
        private bool _needsCleanup = false;
        private int _triggerDepth = 0;

        public IUnRegister Register(Action onEvent)
        {
            if (_triggerDepth > 0)
            {
                if (!_callbacks.Contains(onEvent) && !_pendingAdds.Contains(onEvent))
                {
                    _pendingAdds.Add(onEvent);
                }
            }
            else
            {
                if (!_callbacks.Contains(onEvent))
                {
                    _callbacks.Add(onEvent);
                }
            }

            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action onEvent)
        {
            int index = _callbacks.IndexOf(onEvent);
            if (index >= 0)
            {
                _callbacks[index] = null;
                _needsCleanup = true;
            }
            else if (_triggerDepth > 0)
            {
                index = _pendingAdds.IndexOf(onEvent);
                if (index >= 0)
                {
                    _pendingAdds.RemoveAt(index);
                }
            }
        }

        public void UnRegisterAll()
        {
            if (_triggerDepth > 0)
            {
                _pendingAdds.Clear();
                for (int i = 0; i < _callbacks.Count; i++)
                {
                    _callbacks[i] = null;
                }

                _needsCleanup = true;
            }
            else
            {
                _callbacks.Clear();
            }
        }

        public void Trigger()
        {
            _triggerDepth++;

            foreach (var callback in _callbacks)
            {
                if (callback == null)
                {
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

            _triggerDepth--;

            if (_triggerDepth == 0)
            {
                if (_pendingAdds.Count > 0)
                {
                    foreach (var cb in _pendingAdds)
                    {
                        if (!_callbacks.Contains(cb))
                        {
                            _callbacks.Add(cb);
                        }
                    }

                    _pendingAdds.Clear();
                }

                if (_needsCleanup)
                {
                    _callbacks.RemoveAll(c => c == null);
                    _needsCleanup = false;
                }
            }
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