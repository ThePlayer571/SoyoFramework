using System;
using System.Collections.Generic;

namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    public class EasyEvent
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
    }

    public class EasyEvent<T>
    {
        private Action<T> _onEvent;

        // 懒加载，只有用到前置处理时才分配，节省内存
        private List<Func<T, T>> _beforeProcessList;

        public IReadOnlyList<Func<T, T>> BeforeProcessList =>
            _beforeProcessList ?? (IReadOnlyList<Func<T, T>>)Array.Empty<Func<T, T>>();

        public IUnRegister Register(Action<T> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T> onEvent)
        {
            _onEvent -= onEvent;
        }

        public IUnRegister RegisterBefore(Func<T, T> before)
        {
            _beforeProcessList ??= new List<Func<T, T>>();
            _beforeProcessList.Add(before);
            return new CustomUnRegister(() => _beforeProcessList.Remove(before));
        }

        public void Trigger(in T arg, bool useBeforeProcess = true)
        {
            T newArg = arg;
            if (useBeforeProcess && _beforeProcessList != null)
            {
                foreach (var before in _beforeProcessList)
                {
                    newArg = before(newArg);
                }
            }

            _onEvent?.Invoke(newArg);
        }
    }

    public class EasyEvent<T1, T2>
    {
        private Action<T1, T2> _onEvent;

        // 懒加载，只有用到前置处理时才分配，节省内存
        private List<Func<T1, T2, (T1, T2)>> _beforeProcessList;

        public IReadOnlyList<Func<T1, T2, (T1, T2)>> BeforeProcessList => _beforeProcessList ??
                                                                          (IReadOnlyList<Func<T1, T2, (T1, T2)>>)Array
                                                                              .Empty<Func<T1, T2, (T1, T2)>>();

        public IUnRegister Register(Action<T1, T2> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T1, T2> onEvent)
        {
            _onEvent -= onEvent;
        }

        public IUnRegister RegisterBefore(Func<T1, T2, (T1, T2)> before)
        {
            _beforeProcessList ??= new List<Func<T1, T2, (T1, T2)>>();
            _beforeProcessList.Add(before);
            return new CustomUnRegister(() => _beforeProcessList.Remove(before));
        }

        public void Trigger(in T1 arg1, in T2 arg2, bool useBeforeProcess = true)
        {
            (T1, T2) newArgs = (arg1, arg2);
            if (useBeforeProcess && _beforeProcessList != null)
            {
                foreach (var before in _beforeProcessList)
                {
                    newArgs = before(newArgs.Item1, newArgs.Item2);
                }
            }

            _onEvent?.Invoke(newArgs.Item1, newArgs.Item2);
        }
    }

    public class EasyEvent<T1, T2, T3>
    {
        private Action<T1, T2, T3> _onEvent;

        // 懒加载，只有用到前置处理时才分配，节省内存
        private List<Func<T1, T2, T3, (T1, T2, T3)>> _beforeProcessList;

        public IReadOnlyList<Func<T1, T2, T3, (T1, T2, T3)>> BeforeProcessList => _beforeProcessList ??
            (IReadOnlyList<Func<T1, T2, T3, (T1, T2, T3)>>)Array.Empty<Func<T1, T2, T3, (T1, T2, T3)>>();

        public IUnRegister Register(Action<T1, T2, T3> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T1, T2, T3> onEvent)
        {
            _onEvent -= onEvent;
        }

        public IUnRegister RegisterBefore(Func<T1, T2, T3, (T1, T2, T3)> before)
        {
            _beforeProcessList ??= new List<Func<T1, T2, T3, (T1, T2, T3)>>();
            _beforeProcessList.Add(before);
            return new CustomUnRegister(() => _beforeProcessList.Remove(before));
        }

        public void Trigger(in T1 arg1, in T2 arg2, in T3 arg3, bool useBeforeProcess = true)
        {
            (T1, T2, T3) newArgs = (arg1, arg2, arg3);
            if (useBeforeProcess && _beforeProcessList != null)
            {
                foreach (var before in _beforeProcessList)
                {
                    newArgs = before(newArgs.Item1, newArgs.Item2, newArgs.Item3);
                }
            }

            _onEvent?.Invoke(newArgs.Item1, newArgs.Item2, newArgs.Item3);
        }
    }
}