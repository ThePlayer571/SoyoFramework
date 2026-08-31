using System;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework.Utils
{
    public class AggregateRootGetter<TAggregateRoot> : IViewControllerRule, IDisposable
        where TAggregateRoot : class, IAggregateRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TAggregateRoot> _onAwake;
        private IUnRegister? _aggregateRootRegisteredEvent;

        public AggregateRootGetter(Action<TAggregateRoot> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public AggregateRootGetter(IArchitecture relyingArchitecture, Action<TAggregateRoot> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            var aggregateRoot = this.GetAggregateRoot<TAggregateRoot>();

            if (aggregateRoot != null)
            {
                onAwake(aggregateRoot);
            }
            else
            {
                _aggregateRootRegisteredEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRoot>>(OnAggregateRootRegistered);
            }
        }

        private void OnAggregateRootRegistered(AfterAggregateRootRegistered<TAggregateRoot> e)
        {
            _aggregateRootRegisteredEvent?.UnRegister();
            _aggregateRootRegisteredEvent = null;
            _onAwake(e.AggregateRoot);
        }


        public void Dispose()
        {
            _aggregateRootRegisteredEvent?.UnRegister();
            _aggregateRootRegisteredEvent = null;
        }
    }

    public class AggregateRootGetter<TAggregateRootA, TAggregateRootB> : IViewControllerRule, IDisposable
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TAggregateRootA, TAggregateRootB> _onAwake;
        private TAggregateRootA? _aggregateRootA;
        private TAggregateRootB? _aggregateRootB;
        private IUnRegister? _aggregateRootAEvent;
        private IUnRegister? _aggregateRootBEvent;

        public AggregateRootGetter(Action<TAggregateRootA, TAggregateRootB> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public AggregateRootGetter(IArchitecture relyingArchitecture, Action<TAggregateRootA, TAggregateRootB> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            _aggregateRootA = this.GetAggregateRoot<TAggregateRootA>();
            _aggregateRootB = this.GetAggregateRoot<TAggregateRootB>();

            if (_aggregateRootA != null && _aggregateRootB != null)
            {
                onAwake(_aggregateRootA, _aggregateRootB);
                return;
            }

            if (_aggregateRootA == null)
            {
                _aggregateRootAEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootA>>(OnAggregateRootARegistered);
            }

            if (_aggregateRootB == null)
            {
                _aggregateRootBEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootB>>(OnAggregateRootBRegistered);
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
                _onAwake(_aggregateRootA, _aggregateRootB);
            }
        }

        public void Dispose()
        {
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
        }
    }

    public class AggregateRootGetter<TAggregateRootA, TAggregateRootB, TAggregateRootC> : IViewControllerRule,
        IDisposable
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
        where TAggregateRootC : class, IAggregateRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TAggregateRootA, TAggregateRootB, TAggregateRootC> _onAwake;
        private TAggregateRootA? _aggregateRootA;
        private TAggregateRootB? _aggregateRootB;
        private TAggregateRootC? _aggregateRootC;
        private IUnRegister? _aggregateRootAEvent;
        private IUnRegister? _aggregateRootBEvent;
        private IUnRegister? _aggregateRootCEvent;

        public AggregateRootGetter(Action<TAggregateRootA, TAggregateRootB, TAggregateRootC> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public AggregateRootGetter(IArchitecture relyingArchitecture,
            Action<TAggregateRootA, TAggregateRootB, TAggregateRootC> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            _aggregateRootA = this.GetAggregateRoot<TAggregateRootA>();
            _aggregateRootB = this.GetAggregateRoot<TAggregateRootB>();
            _aggregateRootC = this.GetAggregateRoot<TAggregateRootC>();

            if (_aggregateRootA != null && _aggregateRootB != null && _aggregateRootC != null)
            {
                onAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC);
                return;
            }

            if (_aggregateRootA == null)
            {
                _aggregateRootAEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootA>>(OnAggregateRootARegistered);
            }

            if (_aggregateRootB == null)
            {
                _aggregateRootBEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootB>>(OnAggregateRootBRegistered);
            }

            if (_aggregateRootC == null)
            {
                _aggregateRootCEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootC>>(OnAggregateRootCRegistered);
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
                _onAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC);
            }
        }

        public void Dispose()
        {
            _aggregateRootAEvent?.UnRegister();
            _aggregateRootAEvent = null;
            _aggregateRootBEvent?.UnRegister();
            _aggregateRootBEvent = null;
            _aggregateRootCEvent?.UnRegister();
            _aggregateRootCEvent = null;
        }
    }

    public class AggregateRootGetter<TAggregateRootA, TAggregateRootB, TAggregateRootC, TAggregateRootD> :
        IViewControllerRule,
        IDisposable
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
        where TAggregateRootC : class, IAggregateRoot
        where TAggregateRootD : class, IAggregateRoot
    {
        public IArchitecture RelyingArchitecture => _relyingArchitecture;

        private readonly IArchitecture _relyingArchitecture;
        private readonly Action<TAggregateRootA, TAggregateRootB, TAggregateRootC, TAggregateRootD> _onAwake;
        private TAggregateRootA? _aggregateRootA;
        private TAggregateRootB? _aggregateRootB;
        private TAggregateRootC? _aggregateRootC;
        private TAggregateRootD? _aggregateRootD;
        private IUnRegister? _aggregateRootAEvent;
        private IUnRegister? _aggregateRootBEvent;
        private IUnRegister? _aggregateRootCEvent;
        private IUnRegister? _aggregateRootDEvent;

        public AggregateRootGetter(Action<TAggregateRootA, TAggregateRootB, TAggregateRootC, TAggregateRootD> onAwake)
            : this(
                GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                    "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架" +
                    $"如果你不打算使用 GlobalArchitecture，请传入正确的 IArchitecture 实例。"),
                onAwake)
        {
        }

        public AggregateRootGetter(IArchitecture relyingArchitecture,
            Action<TAggregateRootA, TAggregateRootB, TAggregateRootC, TAggregateRootD> onAwake)
        {
            _relyingArchitecture = relyingArchitecture;
            _onAwake = onAwake;

            _aggregateRootA = this.GetAggregateRoot<TAggregateRootA>();
            _aggregateRootB = this.GetAggregateRoot<TAggregateRootB>();
            _aggregateRootC = this.GetAggregateRoot<TAggregateRootC>();
            _aggregateRootD = this.GetAggregateRoot<TAggregateRootD>();

            if (_aggregateRootA != null && _aggregateRootB != null && _aggregateRootC != null && _aggregateRootD != null)
            {
                onAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC, _aggregateRootD);
                return;
            }

            if (_aggregateRootA == null)
            {
                _aggregateRootAEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootA>>(OnAggregateRootARegistered);
            }

            if (_aggregateRootB == null)
            {
                _aggregateRootBEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootB>>(OnAggregateRootBRegistered);
            }

            if (_aggregateRootC == null)
            {
                _aggregateRootCEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootC>>(OnAggregateRootCRegistered);
            }

            if (_aggregateRootD == null)
            {
                _aggregateRootDEvent =
                    this.RegisterEvent<AfterAggregateRootRegistered<TAggregateRootD>>(OnAggregateRootDRegistered);
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
                _onAwake(_aggregateRootA, _aggregateRootB, _aggregateRootC, _aggregateRootD);
            }
        }

        public void Dispose()
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
    }
}
