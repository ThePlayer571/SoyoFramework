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
        /// 聚合根注销回调。回调时已从注册表移除，事件系统在回调后清空。
        /// </summary>
        protected abstract void OnUnregister();
    }
}
