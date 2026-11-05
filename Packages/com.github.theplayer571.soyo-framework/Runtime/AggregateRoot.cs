using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly TypeEventSystem _eventSystem = new();

        protected AggregateRoot()
        {
        }

        IAggregateRoot IAggregateMember.AggregateRoot => this;

        protected void SendEvent<T>() where T : new() => ((IAggregateRoot)this).SendEvent<T>();

        protected void SendEvent<T>(in T e) => ((IAggregateRoot)this).SendEvent(in e);

        protected bool RegisterAggregateRoot<T>(T aggregateRoot) where T : class, IAggregateRoot
            => Architecture.Instance.RegisterAggregateRoot(aggregateRoot, this);

        protected bool UnregisterAggregateRoot<T>() where T : class, IAggregateRoot
            => Architecture.Instance.UnregisterAggregateRoot<T>(this);

        protected T? GetAggregateRoot<T>() where T : class, IAggregateRoot
            => Architecture.Instance.GetAggregateRoot<T>(this);

        protected IAggregateRootHandle<T>? GetAggregateRootHandle<T>() where T : class, IAggregateRoot
            => Architecture.Instance.GetAggregateRootHandle<T>(this);

        IUnRegister IAggregateRoot.RegisterEvent<T>(Action<T> onEvent)
        {
            return _eventSystem.Register(onEvent);
        }

        void IAggregateRoot.SendEvent<T>()
        {
            _eventSystem.Call<T>();
        }

        void IAggregateRoot.SendEvent<T>(in T e)
        {
            _eventSystem.Call(in e);
        }

        void IAggregateRoot.OnUnregister()
        {
            OnUnregister();
            _eventSystem.Clear();
        }

        /// <summary>
        /// 聚合根注销回调。回调期间仍在注册表中，成功返回后清空事件系统。
        /// </summary>
        protected abstract void OnUnregister();
    }
}
