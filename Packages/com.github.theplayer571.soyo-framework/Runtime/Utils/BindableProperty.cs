using System;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework.Utils
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
        void ForceTrigger();
        void UnRegisterAll();
    }

    [Serializable]
    public partial class BindableProperty<T> : IBindableProperty<T>
    {
        #region 接口实现：IReadOnlyBindableProperty

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

        #endregion

        #region 接口实现：IBindableProperty

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

        public void SetValueWithoutTrigger(T value)
        {
            _value = value;
        }

        public void ForceTrigger()
        {
            _valueChangeEvent?.Trigger(_value);
        }

        public void UnRegisterAll()
        {
            _valueChangeEvent.UnRegisterAll();
        }

        #endregion

        private readonly EasyEvent<T> _valueChangeEvent = new EasyEvent<T>();
        private T _value;

        public BindableProperty(T initialValue)
        {
            _value = initialValue;
        }

        #region 内部类

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

        #endregion
    }

    public partial class BindableProperty<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private T _serializedValue = default!;

        public void OnBeforeSerialize()
        {
            _serializedValue = _value!;
        }

        public void OnAfterDeserialize()
        {
            // 事件会在CustomPropertyDrawer里触发
            _value = _serializedValue;
        }
    }
}