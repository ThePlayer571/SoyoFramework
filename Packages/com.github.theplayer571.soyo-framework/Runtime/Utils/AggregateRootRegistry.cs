using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        internal bool TryRegister<T>(T aggregateRoot, IAggregateMember? requester) where T : class, IAggregateRoot
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

            if (_container.FindKeyByReference(aggregateRoot) != null)
            {
                $"聚合实例 {aggregateRoot.GetType().Name} 已使用其他 key 注册。".LogError();
                return false;
            }

            if (!CanManage(requester, key))
            {
                return false;
            }

            if (!AggregateRelations.TryGet(aggregateRoot.GetType(), out var relations, out var error))
            {
                error.LogError();
                return false;
            }

            foreach (var relation in relations)
            {
                if (relation.Value == AggregateRelationKind.Requires && _container.Get(relation.Key) == null)
                {
                    $"注册 {key.Name} 之前，需要确保 {relation.Key.Name} 已注册。".LogError();
                    return false;
                }

                if (relation.Value == AggregateRelationKind.Manages &&
                    FindManager(relation.Key) is { } manager)
                {
                    ($"{relation.Key.Name} 已有其他管理者实例：{manager.GetType().FullName} " +
                     $"(实例标识 0x{RuntimeHelpers.GetHashCode(manager):X})。").LogError();
                    return false;
                }
            }

            _finalizedUnregisterTypes.Remove(key);
            _container.Register(key, aggregateRoot);

            var handle = GetOrCreateHandle<T>();
            handle.Attach(aggregateRoot);
            handle.NotifyChanged();
            return true;
        }

        internal IAggregateRootHandle<T>? GetHandle<T>(IAggregateMember? requester)
            where T : class, IAggregateRoot
        {
            if (requester != null && !HasRelation(requester, typeof(T), AggregateRelationKind.Observes))
            {
                return null;
            }

            return GetOrCreateHandle<T>();
        }

        internal T? GetRoot<T>(IAggregateMember? requester) where T : class, IAggregateRoot
        {
            if (requester == null)
            {
                $"外部调用者不能直接读取 {typeof(T).Name}。".LogError();
                return null;
            }

            if (!TryGetRequesterRelations(requester, out var relations) ||
                !relations.TryGetValue(typeof(T), out var relation) ||
                (relation != AggregateRelationKind.Requires && relation != AggregateRelationKind.Manages))
            {
                $"当前聚合无权直接读取 {typeof(T).Name}。".LogError();
                return null;
            }

            return _container.Get(typeof(T)) as T;
        }

        private bool HasRelation(IAggregateMember requester, Type target, AggregateRelationKind kind)
        {
            if (TryGetRequesterRelations(requester, out var relations) &&
                relations.TryGetValue(target, out var actual) && actual == kind)
            {
                return true;
            }

            $"当前聚合没有对 {target.Name} 的 {kind} 权限。".LogError();
            return false;
        }

        private bool TryGetRequesterRelations(IAggregateMember requester,
            out IReadOnlyDictionary<Type, AggregateRelationKind> relations)
        {
            relations = new Dictionary<Type, AggregateRelationKind>();
            IAggregateRoot root;
            try
            {
                root = requester.AggregateRoot;
            }
            catch (Exception exception)
            {
                $"调用者的聚合根不可用：{exception.Message}".LogError();
                return false;
            }

            if (root == null || _container.FindKeyByReference(root) == null)
            {
                $"调用者的聚合根 {root?.GetType().Name ?? "null"} 尚未注册。".LogError();
                return false;
            }

            if (!AggregateRelations.TryGet(root.GetType(), out relations, out var error))
            {
                error.LogError();
                return false;
            }

            return true;
        }

        private IAggregateRoot? FindManager(Type target)
        {
            foreach (var key in _container.GetAllKeys())
            {
                var root = (IAggregateRoot)_container.Get(key)!;
                if (AggregateRelations.TryGet(root.GetType(), out var relations, out _) &&
                    relations.TryGetValue(target, out var kind) && kind == AggregateRelationKind.Manages)
                {
                    return root;
                }
            }

            return null;
        }

        private bool CanManage(IAggregateMember? requester, Type target)
        {
            var manager = FindManager(target);
            if (requester == null)
            {
                if (manager == null) return true;
                $"{target.Name} 已由 {manager.GetType().Name} 管理，外部调用无权操作。".LogError();
                return false;
            }

            if (!HasRelation(requester, target, AggregateRelationKind.Manages)) return false;
            if (manager == null || ReferenceEquals(manager, requester.AggregateRoot)) return true;
            $"{target.Name} 已由其他聚合实例管理。".LogError();
            return false;
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

        internal bool RequestUnregister<T>(IAggregateMember? requester) where T : class, IAggregateRoot
        {
            var key = typeof(T);
            if (_container.Get(key) == null)
            {
                $"尝试注销未注册的聚合：{key.Name}。".LogError();
                return false;
            }

            if (!CanManage(requester, key) || HasRequiredDependents(key)) return false;
            if (_activeUnregisterType == key) return false;
            if (_pendingUnregisterTypes.Contains(key)) return true;
            if (!TryQueueUnregister(key)) return false;
            ProcessPendingUnregisters();
            return true;
        }

        private bool HasRequiredDependents(Type target)
        {
            foreach (var key in _container.GetAllKeys())
            {
                var root = (IAggregateRoot)_container.Get(key)!;
                if (AggregateRelations.TryGet(root.GetType(), out var relations, out _) &&
                    relations.TryGetValue(target, out var kind) && kind == AggregateRelationKind.Requires)
                {
                    $"{target.Name} 仍被已注册的 {key.Name} 依赖，不能注销。".LogError();
                    return true;
                }
            }

            return false;
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

                    if (HasRequiredDependents(aggregateRootType)) continue;
                    _activeUnregisterType = aggregateRootType;
                    try
                    {
                        aggregateRoot.OnUnregister();
                    }
                    finally
                    {
                        _activeUnregisterType = null;
                    }

                    _container.Unregister(aggregateRootType);
                    _finalizedUnregisterTypes.Add(aggregateRootType);
                    if (_handles.TryGetValue(aggregateRootType, out var handle))
                    {
                        handle.Detach(aggregateRoot);
                        handle.NotifyChanged();
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
