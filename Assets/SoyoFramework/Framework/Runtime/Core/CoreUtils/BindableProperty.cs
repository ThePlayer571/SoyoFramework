using System;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.CoreUtils
{
    public interface IReadOnlyBindableProperty<out T>
    {
        T Value { get; }
        IUnRegister Register(Action<T> onValueChanged);
        void UnRegister(Action<T> onValueChanged);
        IUnRegister RegisterWithInitValue(Action<T> onValueChanged);
    }

    public interface IBindableProperty<T> : IReadOnlyBindableProperty<T>
    {
        new T Value { get; set; }
        void SetValueWithoutTrigger(T value);
        IUnRegister RegisterBefore(Func<T, T> onBeforeValueChanged);
    }

    [Serializable]
    public partial class BindableProperty<T> : IBindableProperty<T>
    {
        private EasyEvent<T> _valueChangeEvent = new();
        private T _value;

        public BindableProperty(T initialValue = default)
        {
            _value = initialValue;
            _serializedValue = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                T newValue = value;
                // 通过 _valueChangeEvent 的 RegisterBefore 机制处理新值
                if (_valueChangeEvent != null)
                {
                    foreach (var beforeProcess in _valueChangeEvent.BeforeProcessList)
                    {
                        newValue = beforeProcess(newValue);
                    }
                }

                if (!Equals(_value, newValue))
                {
                    _value = newValue;
                    _valueChangeEvent.Trigger(_value, useBeforeProcess: false);
                }
            }
        }

        IUnRegister IReadOnlyBindableProperty<T>.Register(Action<T> onValueChanged)
        {
            return _valueChangeEvent.Register(onValueChanged);
        }

        void IReadOnlyBindableProperty<T>.UnRegister(Action<T> onValueChanged)
        {
            _valueChangeEvent.UnRegister(onValueChanged);
        }

        IUnRegister IReadOnlyBindableProperty<T>.RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(_value);
            return _valueChangeEvent.Register(onValueChanged);
        }

        public IUnRegister Register(Action<T> onValueChanged)
        {
            return _valueChangeEvent.Register(onValueChanged);
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            _valueChangeEvent.UnRegister(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(_value);
            return _valueChangeEvent.Register(onValueChanged);
        }

        public void SetValueWithoutTrigger(T value)
        {
            _value = value;
        }

        public IUnRegister RegisterBefore(Func<T, T> onBeforeValueChanged)
        {
            return _valueChangeEvent.RegisterBefore(onBeforeValueChanged);
        }

        private class ActionUnRegister : IUnRegister
        {
            private Action _onUnRegister;
            private bool _isUnregistered;

            public ActionUnRegister(Action onUnRegister)
            {
                _onUnRegister = onUnRegister;
            }

            public void UnRegister()
            {
                if (!_isUnregistered)
                {
                    _onUnRegister?.Invoke();
                    _isUnregistered = true;
                }
            }
        }
    }

    public partial class BindableProperty<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private T _serializedValue;

        public void OnBeforeSerialize()
        {
            _serializedValue = _value;
        }

        public void OnAfterDeserialize()
        {
            this.Value = _serializedValue;
        }
    }
}