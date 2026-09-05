using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.Utils
{
    internal sealed class AggregateRootRegistry
    {
        // Aggregate实例所有权拥有者
        private readonly SimpleIOCContainer _container = new();
        
        private readonly Queue<Type> _unregisterQueue = new();
        private readonly HashSet<Type> _queuedUnregisterTypes = new();
        private readonly HashSet<Type> _processedUnregisterTypes = new();
        private bool _isUnregistering;
        private Type? _currentUnregisterType;

        internal bool TryRegister(Type key, IAggregateRoot aggregateRoot)
        {
            if (_isUnregistering)
            {
                $"禁止在 UnregisterAggregateRoot 期间注册 {nameof(IAggregateRoot)}: {key.Name}".LogError();
                return false;
            }

            if (_container.Get(key) != null)
            {
                $"禁止重复注册 {nameof(IAggregateRoot)}: {key.Name}".LogError();
                return false;
            }

            // 已注销的 key 再次注册时代表一个新实例，允许它重新进入注销流程。
            _processedUnregisterTypes.Remove(key);
            _container.Register(key, aggregateRoot);
            return true;
        }

        internal object? Get(Type key) => _container.Get(key);

        internal void RequestUnregister(Type key)
        {
            QueueUnregister(key);
            ProcessUnregisterQueue();
        }

        private bool QueueUnregister(Type aggregateRootType)
        {
            if (_currentUnregisterType == aggregateRootType ||
                _processedUnregisterTypes.Contains(aggregateRootType) ||
                !_queuedUnregisterTypes.Add(aggregateRootType))
            {
                return false;
            }

            _unregisterQueue.Enqueue(aggregateRootType);
            return true;
        }

        private void ProcessUnregisterQueue()
        {
            if (_isUnregistering)
            {
                return;
            }

            _isUnregistering = true;
            var completed = false;
            try
            {
                while (_unregisterQueue.Count > 0)
                {
                    var aggregateRootType = _unregisterQueue.Dequeue();
                    _queuedUnregisterTypes.Remove(aggregateRootType);

                    var aggregateRoot = _container.Get(aggregateRootType) as IAggregateRoot;
                    if (aggregateRoot == null)
                    {
                        $"尝试注销未注册的{nameof(IAggregateRoot)}: {aggregateRootType.Name}".LogError();
                        continue;
                    }

                    // 先从容器移除，防止回调中再次找到并注销自身。
                    _container.Unregister(aggregateRootType);
                    _currentUnregisterType = aggregateRootType;
                    try
                    {
                        aggregateRoot.OnUnregister();
                    }
                    finally
                    {
                        _currentUnregisterType = null;
                        _processedUnregisterTypes.Add(aggregateRootType);
                    }
                }

                completed = true;
            }
            finally
            {
                // 回调异常时保留原始异常向外传播，但不在之后的调用中
                // 自动继续执行当时尚未处理的注销请求。
                if (!completed)
                {
                    _unregisterQueue.Clear();
                    _queuedUnregisterTypes.Clear();
                }

                _isUnregistering = false;
            }
        }
    }
}
