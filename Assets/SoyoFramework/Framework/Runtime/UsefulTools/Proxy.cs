namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    public class Proxy<T> where T : class
    {
        public T Get => _isValid ? _instance : null;
        public bool IsValid => _isValid;

        private T _instance;
        private bool _isValid = false;

        internal Proxy(T instance)
        {
            SetInstance(instance);
        }

        internal void SetInstance(T instance)
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