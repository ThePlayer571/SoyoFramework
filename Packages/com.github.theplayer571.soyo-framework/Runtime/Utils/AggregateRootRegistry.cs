using System;
using System.Collections.Generic;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.Utils
{
    internal sealed class AggregateRootRegistry
    {
        // AggregateRoot 实例所有权拥有者，承诺内部示例为已注册状态
        private readonly SimpleIOCContainer _container = new();

        // 
        private readonly Queue<Type> _pendingUnregisterQueue = new();
        private readonly HashSet<Type> _pendingUnregisterTypes = new(); // 弥补 _pendingUnregisterQueue 不能去重的缺陷

        // 临时的数据结构。记录注销流程已走完的类型
        private readonly HashSet<Type> _finalizedUnregisterTypes = new();

        //
        private bool _isProcessingUnregisterQueue;
        private Type? _activeUnregisterType; // 当前正在执行 OnUnregister 的类型。该类型已先从 _container 移除。

        #region 注册操作

        internal bool TryRegister(Type key, IAggregateRoot aggregateRoot)
        {
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

            // 重新注册代表一个新实例，允许该类型再次进入注销流程。
            _finalizedUnregisterTypes.Remove(key);
            _container.Register(key, aggregateRoot);
            return true;
        }

        internal object? Get(Type key) => _container.Get(key);

        #endregion

        #region 注销操作

        internal void RequestUnregister(Type key)
        {
            TryQueueUnregister(key);
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

            // 
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
                    // 移出待处理
                    var aggregateRootType = _pendingUnregisterQueue.Dequeue();
                    _pendingUnregisterTypes.Remove(aggregateRootType);

                    var aggregateRoot = _container.Get(aggregateRootType) as IAggregateRoot;
                    if (aggregateRoot == null)
                    {
                        $"尝试注销未注册的{nameof(IAggregateRoot)}: {aggregateRootType.Name}".LogError();
                        continue;
                    }

                    // 先从容器移除，防止回调中再次找到并注销自身。
                    _container.Unregister(aggregateRootType);
                    _activeUnregisterType = aggregateRootType;
                    try
                    {
                        aggregateRoot.OnUnregister();
                    }
                    finally
                    {
                        // 无论回调是否抛出异常，都认为该类型已完成注销。
                        _activeUnregisterType = null;
                        _finalizedUnregisterTypes.Add(aggregateRootType);
                    }
                }

                completed = true;
            }
            finally
            {
                if (!completed) // 如果意外终止了，清除后续的注销任务
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