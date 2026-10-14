using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.Utils
{
    internal sealed class AggregateRootRegistry
    {
        private readonly SimpleIOCContainer _container = new();
        private readonly Dictionary<Type, IAggregateRootHandleInternal> _handles = new();

        private readonly Queue<Type> _pendingUnregisterQueue = new();
        private readonly HashSet<Type> _pendingUnregisterTypes = new();
        private readonly HashSet<Type> _finalizedUnregisterTypes = new();

        private bool _isProcessingUnregisterQueue;
        private Type? _activeUnregisterType;

        #region 注册操作

        internal bool TryRegister<T>(T aggregateRoot) where T : class, IAggregateRoot
        {
            var key = typeof(T);

            if (_isProcessingUnregisterQueue)
            {
                $"禁止在 UnregisterAggregateRoot 期间注册 {nameof(IAggregateRoot)}: {key.Name}".LogError();
                return false;
            }

            if (_container.Get(key) != null)
            {
                $"禁止重复注册 {nameof(IAggregateRoot)}: {key.Name}".LogError();
                return false;
            }

            _finalizedUnregisterTypes.Remove(key);
            _container.Register(key, aggregateRoot);

            var handle = GetOrCreateHandle<T>();
            handle.Attach(aggregateRoot);
            handle.NotifyChanged();
            return true;
        }

        internal IAggregateRootHandle<T> GetHandle<T>() where T : class, IAggregateRoot
        {
            return GetOrCreateHandle<T>();
        }

        private AggregateRootHandle<T> GetOrCreateHandle<T>() where T : class, IAggregateRoot
        {
            var key = typeof(T);
            if (_handles.TryGetValue(key, out var handle))
            {
                return (AggregateRootHandle<T>)handle;
            }

            var typedHandle = new AggregateRootHandle<T>();
            _handles.Add(key, typedHandle);
            return typedHandle;
        }

        #endregion

        #region 注销操作

        internal void RequestUnregister<T>() where T : class, IAggregateRoot
        {
            TryQueueUnregister(typeof(T));
            ProcessPendingUnregisters();
        }

        private bool TryQueueUnregister(Type aggregateRootType)
        {
            var canQueue = _activeUnregisterType != aggregateRootType &&
                           !_finalizedUnregisterTypes.Contains(aggregateRootType) &&
                           !_pendingUnregisterTypes.Contains(aggregateRootType);
            if (!canQueue)
            {
                return false;
            }

            _pendingUnregisterTypes.Add(aggregateRootType);
            _pendingUnregisterQueue.Enqueue(aggregateRootType);
            return true;
        }

        private void ProcessPendingUnregisters()
        {
            if (_isProcessingUnregisterQueue)
            {
                return;
            }

            _isProcessingUnregisterQueue = true;
            var completed = false;
            try
            {
                while (_pendingUnregisterQueue.Count > 0)
                {
                    var aggregateRootType = _pendingUnregisterQueue.Dequeue();
                    _pendingUnregisterTypes.Remove(aggregateRootType);

                    var aggregateRoot = _container.Get(aggregateRootType) as IAggregateRoot;
                    if (aggregateRoot == null)
                    {
                        $"尝试注销未注册的{nameof(IAggregateRoot)}: {aggregateRootType.Name}".LogError();
                        continue;
                    }

                    _container.Unregister(aggregateRootType);
                    _handles.TryGetValue(aggregateRootType, out var handle);
                    handle?.Detach(aggregateRoot);
                    _activeUnregisterType = aggregateRootType;
                    try
                    {
                        aggregateRoot.OnUnregister();
                    }
                    finally
                    {
                        _activeUnregisterType = null;
                        _finalizedUnregisterTypes.Add(aggregateRootType);
                        handle?.NotifyChanged();
                    }
                }

                completed = true;
            }
            finally
            {
                if (!completed)
                {
                    _pendingUnregisterQueue.Clear();
                    _pendingUnregisterTypes.Clear();
                }

                _isProcessingUnregisterQueue = false;
            }
        }

        #endregion
    }
}
