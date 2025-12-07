namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    public interface IProxy<T> where T : class
    {
        public T Get { get; }
        public bool IsValid { get; }
        internal void SetInstance(T instance);
    }

    internal class Proxy<T> : IProxy<T> where T : class
    {
        public T Get => _isValid ? _instance : null;
        public bool IsValid => _isValid;

        private T _instance;
        private bool _isValid = false;

        internal Proxy(T instance)
        {
            ((IProxy<T>)this).SetInstance(instance);
        }

        void IProxy<T>.SetInstance(T instance)
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
    }
}