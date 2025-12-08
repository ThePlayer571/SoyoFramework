using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            IProxy<T> proxy = new Proxy<T>(instance);
            _container[key] = proxy;
        }

        [return: NotNull]
        public IProxy<T> Get<T>() where T : class
        {
            var key = typeof(T);

            if (_container.TryGetValue(key, out var retInstance))
            {
                return retInstance as IProxy<T> ??
                       throw new Exception($"在ProxyIOCContainer中，Key: {key} 对应的实例无法转换为 IProxy<{key}>");
            }

            // 如果没有找到，创建一个新的Proxy包装类
            IProxy<T> proxy = new Proxy<T>(null);
            _container[key] = proxy;
            return proxy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>迭代器中的值非null</returns>
        [return: NotNull]
        public IEnumerable<IProxy<T>> GetAll<T>() where T : class
        {
            var key = typeof(T);
            return _container
                .Where(kv => key.IsAssignableFrom(kv.Key))
                .Select(kv => kv.Value as IProxy<T>);
        }

        public void Clear() => _container.Clear();
    }
}