using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    public abstract class MonoVController<TAggregateRoot> : MonoVController
        where TAggregateRoot : class, IAggregateRoot
    {
        private IUnRegister? _aggregateRootRegisteredEvent;

        protected void Awake()
        {
            var aggregateRoot = this.GetAggregateRoot<TAggregateRoot>();

            if (aggregateRoot != null)
            {
                OnAwake(aggregateRoot);
            }
            else
            {
                _aggregateRootRegisteredEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRoot>>(OnAggregateRootRegistered);
            }
        }

        private void OnAggregateRootRegistered(AfterAggregateRootRegistered<TAggregateRoot> e)
        {
            _aggregateRootRegisteredEvent?.UnRegister();
            _aggregateRootRegisteredEvent = null;
            OnAwake(e.AggregateRoot);
        }

        protected virtual void OnDestroy()
        {
            _aggregateRootRegisteredEvent?.UnRegister();
            _aggregateRootRegisteredEvent = null;
        }

        protected abstract void OnAwake(TAggregateRoot aggregateRoot);
    }

    public abstract class MonoVController<TAggregateRootA, TAggregateRootB> : MonoVController
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
    {
        private TAggregateRootA? _aggregateRootA;
        private TAggregateRootB? _aggregateRootB;
        private IUnRegister? _aggregateRootAEvent;
        private IUnRegister? _aggregateRootBEvent;

        protected void Awake()
        {
            _aggregateRootA = this.GetAggregateRoot<TAggregateRootA>();
            _aggregateRootB = this.GetAggregateRoot<TAggregateRootB>();

            if (_aggregateRootA != null && _aggregateRootB != null)
            {
                OnAwake(_aggregateRootA, _aggregateRootB);
                return;
            }

            if (_aggregateRootA == null)
            {
                _aggregateRootAEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootA>>(OnAggregateRootARegistered);
            }

            if (_aggregateRootB == null)
            {
                _aggregateRootBEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootB>>(OnAggregateRootBRegistered);
            }
        }

        private void OnAggregateRootARegistered(AfterAggregateRootRegistered<TAggregateRootA> e)
        {
            _aggregateRootA = e.AggregateRoot;
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            TryInvokeOnAwake();
        }

        private void OnAggregateRootBRegistered(AfterAggregateRootRegistered<TAggregateRootB> e)
        {
            _aggregateRootB = e.AggregateRoot;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
            TryInvokeOnAwake();
        }

        private void TryInvokeOnAwake()
        {
            if (_aggregateRootA != null && _aggregateRootB != null)
            {
                OnAwake(_aggregateRootA, _aggregateRootB);
            }
        }

        protected virtual void OnDestroy()
        {
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
        }

        protected abstract void OnAwake(TAggregateRootA aggregateRootA, TAggregateRootB aggregateRootB);
    }

    public abstract class MonoVController<TAggregateRootA, TAggregateRootB, TAggregateRootC> : MonoVController
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
        where TAggregateRootC : class, IAggregateRoot
    {
        private TAggregateRootA? _aggregateRootA;
        private TAggregateRootB? _aggregateRootB;
        private TAggregateRootC? _aggregateRootC;
        private IUnRegister? _aggregateRootAEvent;
        private IUnRegister? _aggregateRootBEvent;
        private IUnRegister? _aggregateRootCEvent;

        protected void Awake()
        {
            _aggregateRootA = this.GetAggregateRoot<TAggregateRootA>();
            _aggregateRootB = this.GetAggregateRoot<TAggregateRootB>();
            _aggregateRootC = this.GetAggregateRoot<TAggregateRootC>();

            if (_aggregateRootA != null && _aggregateRootB != null && _aggregateRootC != null)
            {
                OnAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC);
                return;
            }

            if (_aggregateRootA == null)
            {
                _aggregateRootAEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootA>>(OnAggregateRootARegistered);
            }

            if (_aggregateRootB == null)
            {
                _aggregateRootBEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootB>>(OnAggregateRootBRegistered);
            }

            if (_aggregateRootC == null)
            {
                _aggregateRootCEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootC>>(OnAggregateRootCRegistered);
            }
        }

        private void OnAggregateRootARegistered(AfterAggregateRootRegistered<TAggregateRootA> e)
        {
            _aggregateRootA = e.AggregateRoot;
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            TryInvokeOnAwake();
        }

        private void OnAggregateRootBRegistered(AfterAggregateRootRegistered<TAggregateRootB> e)
        {
            _aggregateRootB = e.AggregateRoot;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
            TryInvokeOnAwake();
        }

        private void OnAggregateRootCRegistered(AfterAggregateRootRegistered<TAggregateRootC> e)
        {
            _aggregateRootC = e.AggregateRoot;
            _aggregateRootCEvent?.UnRegister();
            _aggregateRootCEvent = null;
            TryInvokeOnAwake();
        }

        private void TryInvokeOnAwake()
        {
            if (_aggregateRootA != null && _aggregateRootB != null && _aggregateRootC != null)
            {
                OnAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC);
            }
        }

        protected virtual void OnDestroy()
        {
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
            _aggregateRootCEvent?.UnRegister();
            _aggregateRootCEvent = null;
        }

        protected abstract void OnAwake(TAggregateRootA aggregateRootA, TAggregateRootB aggregateRootB,
            TAggregateRootC aggregateRootC);
    }

    public abstract class MonoVController<TAggregateRootA, TAggregateRootB, TAggregateRootC, TAggregateRootD> :
        MonoVController
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
        where TAggregateRootC : class, IAggregateRoot
        where TAggregateRootD : class, IAggregateRoot
    {
        private TAggregateRootA? _aggregateRootA;
        private TAggregateRootB? _aggregateRootB;
        private TAggregateRootC? _aggregateRootC;
        private TAggregateRootD? _aggregateRootD;
        private IUnRegister? _aggregateRootAEvent;
        private IUnRegister? _aggregateRootBEvent;
        private IUnRegister? _aggregateRootCEvent;
        private IUnRegister? _aggregateRootDEvent;

        protected void Awake()
        {
            _aggregateRootA = this.GetAggregateRoot<TAggregateRootA>();
            _aggregateRootB = this.GetAggregateRoot<TAggregateRootB>();
            _aggregateRootC = this.GetAggregateRoot<TAggregateRootC>();
            _aggregateRootD = this.GetAggregateRoot<TAggregateRootD>();

            if (_aggregateRootA != null && _aggregateRootB != null && _aggregateRootC != null && _aggregateRootD != null)
            {
                OnAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC, _aggregateRootD);
                return;
            }

            if (_aggregateRootA == null)
            {
                _aggregateRootAEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootA>>(OnAggregateRootARegistered);
            }

            if (_aggregateRootB == null)
            {
                _aggregateRootBEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootB>>(OnAggregateRootBRegistered);
            }

            if (_aggregateRootC == null)
            {
                _aggregateRootCEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootC>>(OnAggregateRootCRegistered);
            }

            if (_aggregateRootD == null)
            {
                _aggregateRootDEvent =
                    Architecture.Instance.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootD>>(OnAggregateRootDRegistered);
            }
        }

        private void OnAggregateRootARegistered(AfterAggregateRootRegistered<TAggregateRootA> e)
        {
            _aggregateRootA = e.AggregateRoot;
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            TryInvokeOnAwake();
        }

        private void OnAggregateRootBRegistered(AfterAggregateRootRegistered<TAggregateRootB> e)
        {
            _aggregateRootB = e.AggregateRoot;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
            TryInvokeOnAwake();
        }

        private void OnAggregateRootCRegistered(AfterAggregateRootRegistered<TAggregateRootC> e)
        {
            _aggregateRootC = e.AggregateRoot;
            _aggregateRootCEvent?.UnRegister();
            _aggregateRootCEvent = null;
            TryInvokeOnAwake();
        }

        private void OnAggregateRootDRegistered(AfterAggregateRootRegistered<TAggregateRootD> e)
        {
            _aggregateRootD = e.AggregateRoot;
            _aggregateRootDEvent?.UnRegister();
            _aggregateRootDEvent = null;
            TryInvokeOnAwake();
        }

        private void TryInvokeOnAwake()
        {
            if (_aggregateRootA != null && _aggregateRootB != null && _aggregateRootC != null && _aggregateRootD != null)
            {
                OnAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC, _aggregateRootD);
            }
        }

        protected virtual void OnDestroy()
        {
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
            _aggregateRootCEvent?.UnRegister();
            _aggregateRootCEvent = null;
            _aggregateRootDEvent?.UnRegister();
            _aggregateRootDEvent = null;
        }

        protected abstract void OnAwake(TAggregateRootA aggregateRootA, TAggregateRootB aggregateRootB,
            TAggregateRootC aggregateRootC, TAggregateRootD aggregateRootD);
    }
}
