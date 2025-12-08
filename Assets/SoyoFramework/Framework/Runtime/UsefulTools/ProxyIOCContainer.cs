using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SoyoFramework.Framework.Runtime.UsefulTools
{
    /// <summary>
    /// 特殊的IOC容器，以普通类为key，存储对应的IProxy T
    /// 一旦存储，IProxy实例不会被替换，只会更新其内部的实例引用
    /// </summary>
    internal class ProxyIOCContainer
    {
        private readonly Dictionary<Type, object> _container = new();

        #region 私有辅助方法

        private bool TryGetProxy<T>(out IProxy<T> proxy) where T : class
        {
            var key = typeof(T);
            if (_container.TryGetValue(key, out var value) && value is IProxy<T> typedProxy)
            {
                proxy = typedProxy;
                return true;
            }

            proxy = null;
            return false;
        }

        private IProxy<T> GetOrCreateProxy<T>(T instance = null) where T : class
        {
            if (TryGetProxy<T>(out var existingProxy))
            {
                return existingProxy;
            }

            var newProxy = new Proxy<T>(instance);
            _container[typeof(T)] = newProxy;
            return newProxy;
        }

        private static IProxy<T> CastToProxy<T>(object value, Type key) where T : class
        {
            return value as IProxy<T>
                   ?? throw new InvalidOperationException(
                       $"在ProxyIOCContainer中，Key: {key} 对应的实例无法转换为 IProxy<{typeof(T)}>");
        }

        #endregion

        /// <summary>
        /// 注册实例。若Key已存在则更新内部引用，否则创建新Proxy并添加。
        /// </summary>
        public void Register<T>(T instance) where T : class
        {
            var proxy = GetOrCreateProxy<T>();
            proxy.SetInstance(instance);
        }

        /// <summary>
        /// 取消注册。若Key存在则将内部引用置null，不会移除Proxy对象。
        /// </summary>
        public void UnRegister<T>() where T : class
        {
            if (TryGetProxy<T>(out var proxy))
            {
                proxy.SetInstance(null);
            }
        }

        /// <summary>
        /// 获取Proxy。若Key不存在会自动创建空Proxy并添加，永远不返回null。
        /// </summary>
        [return: NotNull]
        public IProxy<T> Get<T>() where T : class
        {
            return GetOrCreateProxy<T>();
        }

        /// <summary>
        /// 获取所有可分配给T的Proxy。仅返回已存在的，不会创建新Proxy。
        /// </summary>
        [return: NotNull]
        public IEnumerable<IProxy<T>> GetAll<T>() where T : class
        {
            var key = typeof(T);
            return _container
                .Where(kv => key.IsAssignableFrom(kv.Key))
                .Select(kv => CastToProxy<T>(kv.Value, kv.Key));
        }
    }
}