using System;
using System.Collections.Generic;
using System.Linq;

namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    /// <summary>
    /// 特殊的IOC容器，以普通类为key，但是存储的是Proxy包装类
    /// </summary>
    public class ProxyIOCContainer
    {
        private readonly Dictionary<Type, object> _container = new();

        public void Register<T>(T instance) where T : class
        {
            var key = typeof(T);
            var proxy = new Proxy<T>(instance);
            _container[key] = proxy;
        }

        public IProxy<T> Get<T>() where T : class
        {
            var key = typeof(T);

            if (_container.TryGetValue(key, out var retInstance))
            {
                return retInstance as IProxy<T>;
            }

            return null;
        }

        public IEnumerable<IProxy<T>> GetAll<T>() where T : class
        {
            var keyType = typeof(T);
            return _container
                .Where(kv => keyType.IsAssignableFrom(kv.Key))
                .Select(kv => kv.Value as IProxy<T>)
                .Where(proxy => proxy != null);
        }

        public void Clear() => _container.Clear();
    }
}