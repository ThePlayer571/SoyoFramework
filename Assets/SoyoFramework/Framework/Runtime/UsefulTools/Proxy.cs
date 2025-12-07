namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    public interface IProxy<out T> where T : class
    {
        public T Get { get; }
        public bool IsValid { get; }
        internal void SetInstance(object instance);
    }

    internal class Proxy<T> : IProxy<T> where T : class
    {
        public T Get => _isValid ? _instance : null;
        public bool IsValid => _isValid;

        private T _instance;
        private bool _isValid = false;

        internal Proxy(T instance)
        {
            SetInstanceInternal(instance);
        }

        private void SetInstanceInternal(T instance)
        {
            if (instance == null)
            {
                _instance = null;
                _isValid = false;
            }
            else
            {
                _instance = instance;
                _isValid = true;
            }
        }

        void IProxy<T>.SetInstance(object instance)
        {
            SetInstanceInternal(instance as T);
        }
    }
}