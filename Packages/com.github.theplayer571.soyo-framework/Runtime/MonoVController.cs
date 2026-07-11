using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework
{
    public class MonoVController : MonoBehaviour, IMonoVController
    {
        public virtual IArchitecture RelyingArchitecture => GlobalArchitecture.Instance;
    }

    public abstract class MonoVController<TDomainRoot> : MonoVController
        where TDomainRoot : class, IDomainRoot
    {
        private IUnRegister _domainRootRegisteredEvent;

        protected void Awake()
        {
            var domainRoot = this.GetDomainRoot<TDomainRoot>();

            if (domainRoot != null)
            {
                OnAwake(domainRoot);
            }
            else
            {
                _domainRootRegisteredEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRoot>>(OnDomainRootRegistered);
            }
        }

        private void OnDomainRootRegistered(AfterDomainRootRegistered<TDomainRoot> e)
        {
            _domainRootRegisteredEvent?.UnRegister();
            _domainRootRegisteredEvent = null;
            OnAwake(e.DomainRoot);
        }

        protected virtual void OnDestroy()
        {
            _domainRootRegisteredEvent?.UnRegister();
            _domainRootRegisteredEvent = null;
        }

        protected abstract void OnAwake(TDomainRoot domainRoot);
    }

    public abstract class MonoVController<TDomainRootA, TDomainRootB> : MonoVController
        where TDomainRootA : class, IDomainRoot
        where TDomainRootB : class, IDomainRoot
    {
        private TDomainRootA _domainRootA;
        private TDomainRootB _domainRootB;
        private IUnRegister _domainRootAEvent;
        private IUnRegister _domainRootBEvent;

        protected void Awake()
        {
            _domainRootA = this.GetDomainRoot<TDomainRootA>();
            _domainRootB = this.GetDomainRoot<TDomainRootB>();

            if (_domainRootA != null && _domainRootB != null)
            {
                OnAwake(_domainRootA, _domainRootB);
                return;
            }

            if (_domainRootA == null)
            {
                _domainRootAEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootA>>(OnDomainRootARegistered);
            }

            if (_domainRootB == null)
            {
                _domainRootBEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootB>>(OnDomainRootBRegistered);
            }
        }

        private void OnDomainRootARegistered(AfterDomainRootRegistered<TDomainRootA> e)
        {
            _domainRootA = e.DomainRoot;
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            TryInvokeOnAwake();
        }

        private void OnDomainRootBRegistered(AfterDomainRootRegistered<TDomainRootB> e)
        {
            _domainRootB = e.DomainRoot;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
            TryInvokeOnAwake();
        }

        private void TryInvokeOnAwake()
        {
            if (_domainRootA != null && _domainRootB != null)
            {
                OnAwake(_domainRootA, _domainRootB);
            }
        }

        protected virtual void OnDestroy()
        {
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
        }

        protected abstract void OnAwake(TDomainRootA domainRootA, TDomainRootB domainRootB);
    }

    public abstract class MonoVController<TDomainRootA, TDomainRootB, TDomainRootC> : MonoVController
        where TDomainRootA : class, IDomainRoot
        where TDomainRootB : class, IDomainRoot
        where TDomainRootC : class, IDomainRoot
    {
        private TDomainRootA _domainRootA;
        private TDomainRootB _domainRootB;
        private TDomainRootC _domainRootC;
        private IUnRegister _domainRootAEvent;
        private IUnRegister _domainRootBEvent;
        private IUnRegister _domainRootCEvent;

        protected void Awake()
        {
            _domainRootA = this.GetDomainRoot<TDomainRootA>();
            _domainRootB = this.GetDomainRoot<TDomainRootB>();
            _domainRootC = this.GetDomainRoot<TDomainRootC>();

            if (_domainRootA != null && _domainRootB != null && _domainRootC != null)
            {
                OnAwake(_domainRootA, _domainRootB, _domainRootC);
                return;
            }

            if (_domainRootA == null)
            {
                _domainRootAEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootA>>(OnDomainRootARegistered);
            }

            if (_domainRootB == null)
            {
                _domainRootBEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootB>>(OnDomainRootBRegistered);
            }

            if (_domainRootC == null)
            {
                _domainRootCEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootC>>(OnDomainRootCRegistered);
            }
        }

        private void OnDomainRootARegistered(AfterDomainRootRegistered<TDomainRootA> e)
        {
            _domainRootA = e.DomainRoot;
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            TryInvokeOnAwake();
        }

        private void OnDomainRootBRegistered(AfterDomainRootRegistered<TDomainRootB> e)
        {
            _domainRootB = e.DomainRoot;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
            TryInvokeOnAwake();
        }

        private void OnDomainRootCRegistered(AfterDomainRootRegistered<TDomainRootC> e)
        {
            _domainRootC = e.DomainRoot;
            _domainRootCEvent?.UnRegister();
            _domainRootCEvent = null;
            TryInvokeOnAwake();
        }

        private void TryInvokeOnAwake()
        {
            if (_domainRootA != null && _domainRootB != null && _domainRootC != null)
            {
                OnAwake(_domainRootA, _domainRootB, _domainRootC);
            }
        }

        protected virtual void OnDestroy()
        {
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
            _domainRootCEvent?.UnRegister();
            _domainRootCEvent = null;
        }

        protected abstract void OnAwake(TDomainRootA domainRootA, TDomainRootB domainRootB, TDomainRootC domainRootC);
    }

    public abstract class MonoVController<TDomainRootA, TDomainRootB, TDomainRootC, TDomainRootD> : MonoVController
        where TDomainRootA : class, IDomainRoot
        where TDomainRootB : class, IDomainRoot
        where TDomainRootC : class, IDomainRoot
        where TDomainRootD : class, IDomainRoot
    {
        private TDomainRootA _domainRootA;
        private TDomainRootB _domainRootB;
        private TDomainRootC _domainRootC;
        private TDomainRootD _domainRootD;
        private IUnRegister _domainRootAEvent;
        private IUnRegister _domainRootBEvent;
        private IUnRegister _domainRootCEvent;
        private IUnRegister _domainRootDEvent;

        protected void Awake()
        {
            _domainRootA = this.GetDomainRoot<TDomainRootA>();
            _domainRootB = this.GetDomainRoot<TDomainRootB>();
            _domainRootC = this.GetDomainRoot<TDomainRootC>();
            _domainRootD = this.GetDomainRoot<TDomainRootD>();

            if (_domainRootA != null && _domainRootB != null && _domainRootC != null && _domainRootD != null)
            {
                OnAwake(_domainRootA, _domainRootB, _domainRootC, _domainRootD);
                return;
            }

            if (_domainRootA == null)
            {
                _domainRootAEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootA>>(OnDomainRootARegistered);
            }

            if (_domainRootB == null)
            {
                _domainRootBEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootB>>(OnDomainRootBRegistered);
            }

            if (_domainRootC == null)
            {
                _domainRootCEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootC>>(OnDomainRootCRegistered);
            }

            if (_domainRootD == null)
            {
                _domainRootDEvent = this.RegisterEvent<AfterDomainRootRegistered<TDomainRootD>>(OnDomainRootDRegistered);
            }
        }

        private void OnDomainRootARegistered(AfterDomainRootRegistered<TDomainRootA> e)
        {
            _domainRootA = e.DomainRoot;
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            TryInvokeOnAwake();
        }

        private void OnDomainRootBRegistered(AfterDomainRootRegistered<TDomainRootB> e)
        {
            _domainRootB = e.DomainRoot;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
            TryInvokeOnAwake();
        }

        private void OnDomainRootCRegistered(AfterDomainRootRegistered<TDomainRootC> e)
        {
            _domainRootC = e.DomainRoot;
            _domainRootCEvent?.UnRegister();
            _domainRootCEvent = null;
            TryInvokeOnAwake();
        }

        private void OnDomainRootDRegistered(AfterDomainRootRegistered<TDomainRootD> e)
        {
            _domainRootD = e.DomainRoot;
            _domainRootDEvent?.UnRegister();
            _domainRootDEvent = null;
            TryInvokeOnAwake();
        }

        private void TryInvokeOnAwake()
        {
            if (_domainRootA != null && _domainRootB != null && _domainRootC != null && _domainRootD != null)
            {
                OnAwake(_domainRootA, _domainRootB, _domainRootC, _domainRootD);
            }
        }

        protected virtual void OnDestroy()
        {
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
            _domainRootCEvent?.UnRegister();
            _domainRootCEvent = null;
            _domainRootDEvent?.UnRegister();
            _domainRootDEvent = null;
        }

        protected abstract void OnAwake(TDomainRootA domainRootA, TDomainRootB domainRootB, TDomainRootC domainRootC, TDomainRootD domainRootD);
    }
}
