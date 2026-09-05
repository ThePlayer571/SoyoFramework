using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.Utils
{
    // todo：这个类存在巨大缺陷，等后面重构完了来统一处理
    internal sealed class AggregateRootRegistry
    
    {
        // Aggregate实例 所有权拥有者
        private readonly SimpleIOCContainer _container = new();
        
        private readonly Queue<Type> _unregisterQueue = new();
        private readonly HashSet<Type> _queuedUnregisterTypes = new();
        private readonly HashSet<Type> _processedUnregisterTypes = new();
        private bool _isUnregistering;
        private bool _isDeinitializing;
        private Type? _currentUnregisterType;

        internal bool TryRegister(Type key, IAggregateRoot aggregateRoot)
        {
            if (_isUnregistering || _isDeinitializing)
            {
                $"禁止在 UnregisterAggregateRoot 或 Architecture 销毁期间注册 {nameof(IAggregateRoot)}: {key.Name}"
                    .LogError();
                return false;
            }

            // 重新注册同一个 key 后，它代表的是一个新的注册实例，
            // 因此允许该 key 再次进入注销流程。
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

        internal bool BeginDeinitialization()
        {
            if (_isDeinitializing || _isUnregistering)
            {
                "禁止重复进入 Architecture 销毁流程。".LogError();
                return false;
            }

            _isDeinitializing = true;
            return true;
        }

        internal void UnregisterAll()
        {
            foreach (var key in _container.GetAllKeys())
            {
                QueueUnregister(key);
            }

            ProcessUnregisterQueue();
        }

        internal void CompleteDeinitialization()
        {
            _container.Clear();
            _unregisterQueue.Clear();
            _queuedUnregisterTypes.Clear();
            _processedUnregisterTypes.Clear();
            _currentUnregisterType = null;
            _isUnregistering = false;
            _isDeinitializing = false;
        }

        internal void AbortDeinitialization()
        {
            _unregisterQueue.Clear();
            _queuedUnregisterTypes.Clear();
            _currentUnregisterType = null;
            _isUnregistering = false;
            _isDeinitializing = false;
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
                        aggregateRoot.Deinit();
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
