using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    public class DomainRootGetter<TDomainRoot> : IViewControllerRule, IDisposable
        where TDomainRoot : class, IDomainRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TDomainRoot> _onAwake;
        private IUnRegister? _domainRootRegisteredEvent;

        public DomainRootGetter(Action<TDomainRoot> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public DomainRootGetter(IArchitecture relyingArchitecture, Action<TDomainRoot> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            var domainRoot = this.GetDomainRoot<TDomainRoot>();

            if (domainRoot != null)
            {
                onAwake(domainRoot);
            }
            else
            {
                _domainRootRegisteredEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRoot>>(OnDomainRootRegistered);
            }
        }

        private void OnDomainRootRegistered(AfterDomainRootRegistered<TDomainRoot> e)
        {
            _domainRootRegisteredEvent?.UnRegister();
            _domainRootRegisteredEvent = null;
            _onAwake(e.DomainRoot);
        }


        public void Dispose()
        {
            _domainRootRegisteredEvent?.UnRegister();
            _domainRootRegisteredEvent = null;
        }
    }

    public class DomainRootGetter<TDomainRootA, TDomainRootB> : IViewControllerRule, IDisposable
        where TDomainRootA : class, IDomainRoot
        where TDomainRootB : class, IDomainRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TDomainRootA, TDomainRootB> _onAwake;
        private TDomainRootA? _domainRootA;
        private TDomainRootB? _domainRootB;
        private IUnRegister? _domainRootAEvent;
        private IUnRegister? _domainRootBEvent;

        public DomainRootGetter(Action<TDomainRootA, TDomainRootB> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public DomainRootGetter(IArchitecture relyingArchitecture, Action<TDomainRootA, TDomainRootB> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            _domainRootA = this.GetDomainRoot<TDomainRootA>();
            _domainRootB = this.GetDomainRoot<TDomainRootB>();

            if (_domainRootA != null && _domainRootB != null)
            {
                onAwake(_domainRootA, _domainRootB);
                return;
            }

            if (_domainRootA == null)
            {
                _domainRootAEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootA>>(OnDomainRootARegistered);
            }

            if (_domainRootB == null)
            {
                _domainRootBEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootB>>(OnDomainRootBRegistered);
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
                _onAwake(_domainRootA, _domainRootB);
            }
        }

        public void Dispose()
        {
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
        }
    }

    public class DomainRootGetter<TDomainRootA, TDomainRootB, TDomainRootC> : IViewControllerRule, IDisposable
        where TDomainRootA : class, IDomainRoot
        where TDomainRootB : class, IDomainRoot
        where TDomainRootC : class, IDomainRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TDomainRootA, TDomainRootB, TDomainRootC> _onAwake;
        private TDomainRootA? _domainRootA;
        private TDomainRootB? _domainRootB;
        private TDomainRootC? _domainRootC;
        private IUnRegister? _domainRootAEvent;
        private IUnRegister? _domainRootBEvent;
        private IUnRegister? _domainRootCEvent;

        public DomainRootGetter(Action<TDomainRootA, TDomainRootB, TDomainRootC> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public DomainRootGetter(IArchitecture relyingArchitecture,
            Action<TDomainRootA, TDomainRootB, TDomainRootC> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            _domainRootA = this.GetDomainRoot<TDomainRootA>();
            _domainRootB = this.GetDomainRoot<TDomainRootB>();
            _domainRootC = this.GetDomainRoot<TDomainRootC>();

            if (_domainRootA != null && _domainRootB != null && _domainRootC != null)
            {
                onAwake(_domainRootA, _domainRootB, _domainRootC);
                return;
            }

            if (_domainRootA == null)
            {
                _domainRootAEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootA>>(OnDomainRootARegistered);
            }

            if (_domainRootB == null)
            {
                _domainRootBEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootB>>(OnDomainRootBRegistered);
            }

            if (_domainRootC == null)
            {
                _domainRootCEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootC>>(OnDomainRootCRegistered);
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
                _onAwake(_domainRootA, _domainRootB, _domainRootC);
            }
        }

        public void Dispose()
        {
            _domainRootAEvent?.UnRegister();
            _domainRootAEvent = null;
            _domainRootBEvent?.UnRegister();
            _domainRootBEvent = null;
            _domainRootCEvent?.UnRegister();
            _domainRootCEvent = null;
        }
    }

    public class DomainRootGetter<TDomainRootA, TDomainRootB, TDomainRootC, TDomainRootD> : IViewControllerRule,
        IDisposable
        where TDomainRootA : class, IDomainRoot
        where TDomainRootB : class, IDomainRoot
        where TDomainRootC : class, IDomainRoot
        where TDomainRootD : class, IDomainRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TDomainRootA, TDomainRootB, TDomainRootC, TDomainRootD> _onAwake;
        private TDomainRootA? _domainRootA;
        private TDomainRootB? _domainRootB;
        private TDomainRootC? _domainRootC;
        private TDomainRootD? _domainRootD;
        private IUnRegister? _domainRootAEvent;
        private IUnRegister? _domainRootBEvent;
        private IUnRegister? _domainRootCEvent;
        private IUnRegister? _domainRootDEvent;

        public DomainRootGetter(Action<TDomainRootA, TDomainRootB, TDomainRootC, TDomainRootD> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public DomainRootGetter(IArchitecture relyingArchitecture,
            Action<TDomainRootA, TDomainRootB, TDomainRootC, TDomainRootD> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            _domainRootA = this.GetDomainRoot<TDomainRootA>();
            _domainRootB = this.GetDomainRoot<TDomainRootB>();
            _domainRootC = this.GetDomainRoot<TDomainRootC>();
            _domainRootD = this.GetDomainRoot<TDomainRootD>();

            if (_domainRootA != null && _domainRootB != null && _domainRootC != null && _domainRootD != null)
            {
                onAwake(_domainRootA, _domainRootB, _domainRootC, _domainRootD);
                return;
            }

            if (_domainRootA == null)
            {
                _domainRootAEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootA>>(OnDomainRootARegistered);
            }

            if (_domainRootB == null)
            {
                _domainRootBEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootB>>(OnDomainRootBRegistered);
            }

            if (_domainRootC == null)
            {
                _domainRootCEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootC>>(OnDomainRootCRegistered);
            }

            if (_domainRootD == null)
            {
                _domainRootDEvent =
                    this.RegisterEvent<AfterDomainRootRegistered<TDomainRootD>>(OnDomainRootDRegistered);
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
                _onAwake(_domainRootA, _domainRootB, _domainRootC, _domainRootD);
            }
        }

        public void Dispose()
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
    }
}