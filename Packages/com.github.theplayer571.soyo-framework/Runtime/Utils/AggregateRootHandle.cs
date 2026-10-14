using System;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework.Utils
{
    internal interface IAggregateRootHandleInternal
    {
        void Attach(IAggregateRoot aggregateRoot);
        void Detach(IAggregateRoot aggregateRoot);
        void NotifyChanged();
    }

    internal sealed class AggregateRootHandle<T> : IAggregateRootHandle<T>, IAggregateRootHandleInternal
        where T : class, IAggregateRoot
    {
        private readonly EasyEvent<T?> _changedEvent = new();
        private T? _current;

        public T? Current => _current;

        public bool TryGet(out T? aggregateRoot)
        {
            aggregateRoot = _current;
            return aggregateRoot != null;
        }

        public IUnRegister RegisterChanged(Action<T?> onChanged)
        {
            return _changedEvent.Register(onChanged);
        }

        public void Attach(IAggregateRoot aggregateRoot)
        {
            _current = (T)aggregateRoot;
        }

        public void Detach(IAggregateRoot aggregateRoot)
        {
            if (ReferenceEquals(_current, aggregateRoot))
            {
                _current = null;
            }
        }

        public void NotifyChanged()
        {
            var current = _current;
            _changedEvent.Trigger(in current);
        }
    }
}
