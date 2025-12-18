using System;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
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
    }

    [Serializable]
    public partial class BindableProperty<T> : IBindableProperty<T>
    {
        private EasyEvent<T> _valueChangeEvent;
        private T _value;

        public BindableProperty(T initialValue)
        {
            _valueChangeEvent = new EasyEvent<T>();
            _value = initialValue;
        }

        public BindableProperty()
        {
            _valueChangeEvent = new EasyEvent<T>();
            _value = default;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    _valueChangeEvent.Trigger(_value);
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
            // 事件会在CustomPropertyDrawer中触发
            _value = _serializedValue;
        }
    }
}