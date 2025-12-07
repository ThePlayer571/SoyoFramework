using System;
using System.Collections.Generic;
using System.Linq;

namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    
    public class SimpleIOCContainer
    {
        private readonly Dictionary<Type, object> _container = new();

        public void Register<T>(T instance)
        {
            var key = typeof(T);

            _container[key] = instance;
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (_container.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            var keyType = typeof(T);
            return _container.Where(kv => keyType.IsAssignableFrom(kv.Key)).Select(kv => kv.Value as T);
        }

        public void Clear() => _container.Clear();
    }
}