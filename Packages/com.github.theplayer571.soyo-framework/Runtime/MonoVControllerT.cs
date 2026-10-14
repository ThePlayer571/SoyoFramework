using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;

namespace SoyoFramework
{
    public abstract class MonoVController<TAggregateRoot> : MonoVController
        where TAggregateRoot : class, IAggregateRoot
    {
        private IAggregateRootHandle<TAggregateRoot> _aggregateRootHandle = null!;
        private IUnRegister? _aggregateRootChanged;
        private bool _isAvailable;

        protected virtual void Awake()
        {
            _aggregateRootHandle = this.GetAggregateRootHandle<TAggregateRoot>();
            _aggregateRootChanged = _aggregateRootHandle.RegisterChanged(OnAggregateRootChanged);
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRoot? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            if (_aggregateRootHandle.TryGet(out var aggregateRoot))
            {
                if (!_isAvailable)
                {
                    _isAvailable = true;
                    OnAvailable(aggregateRoot!);
                }

                return;
            }

            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }

            _aggregateRootChanged?.UnRegister();
            _aggregateRootChanged = null;
        }

        /// <summary>
        /// 依赖的 AggregateRoot 可用时调用一次；若 AggregateRoot 失效后恢复，会再次调用。
        /// </summary>
        protected abstract void OnAvailable(TAggregateRoot aggregateRoot);

        /// <summary>
        /// 依赖的 AggregateRoot 失效时调用一次；组件销毁时若仍处于可用状态，也会调用一次。
        /// </summary>
        protected abstract void OnUnavailable();
    }

    public abstract class MonoVController<TAggregateRootA, TAggregateRootB> : MonoVController
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
    {
        private IAggregateRootHandle<TAggregateRootA> _aggregateRootAHandle = null!;
        private IAggregateRootHandle<TAggregateRootB> _aggregateRootBHandle = null!;
        private IUnRegister? _aggregateRootAChanged;
        private IUnRegister? _aggregateRootBChanged;
        private bool _isAvailable;

        protected virtual void Awake()
        {
            _aggregateRootAHandle = this.GetAggregateRootHandle<TAggregateRootA>();
            _aggregateRootBHandle = this.GetAggregateRootHandle<TAggregateRootB>();
            _aggregateRootAChanged = _aggregateRootAHandle.RegisterChanged(OnAggregateRootChanged);
            _aggregateRootBChanged = _aggregateRootBHandle.RegisterChanged(OnAggregateRootChanged);
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootA? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootB? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            var aAvailable = _aggregateRootAHandle.TryGet(out var aggregateRootA);
            var bAvailable = _aggregateRootBHandle.TryGet(out var aggregateRootB);

            if (aAvailable && bAvailable)
            {
                if (!_isAvailable)
                {
                    _isAvailable = true;
                    OnAvailable(aggregateRootA!, aggregateRootB!);
                }

                return;
            }

            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }

            _aggregateRootAChanged?.UnRegister();
            _aggregateRootAChanged = null;
            _aggregateRootBChanged?.UnRegister();
            _aggregateRootBChanged = null;
        }

        /// <summary>
        /// 所有依赖的 AggregateRoot 可用时调用一次；若 AggregateRoot 失效后恢复，会再次调用。
        /// </summary>
        protected abstract void OnAvailable(TAggregateRootA aggregateRootA, TAggregateRootB aggregateRootB);

        /// <summary>
        /// 任一依赖的 AggregateRoot 失效时调用一次；组件销毁时若仍处于可用状态，也会调用一次。
        /// </summary>
        protected abstract void OnUnavailable();
    }

    public abstract class MonoVController<TAggregateRootA, TAggregateRootB, TAggregateRootC> : MonoVController
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
        where TAggregateRootC : class, IAggregateRoot
    {
        private IAggregateRootHandle<TAggregateRootA> _aggregateRootAHandle = null!;
        private IAggregateRootHandle<TAggregateRootB> _aggregateRootBHandle = null!;
        private IAggregateRootHandle<TAggregateRootC> _aggregateRootCHandle = null!;
        private IUnRegister? _aggregateRootAChanged;
        private IUnRegister? _aggregateRootBChanged;
        private IUnRegister? _aggregateRootCChanged;
        private bool _isAvailable;

        protected virtual void Awake()
        {
            _aggregateRootAHandle = this.GetAggregateRootHandle<TAggregateRootA>();
            _aggregateRootBHandle = this.GetAggregateRootHandle<TAggregateRootB>();
            _aggregateRootCHandle = this.GetAggregateRootHandle<TAggregateRootC>();
            _aggregateRootAChanged = _aggregateRootAHandle.RegisterChanged(OnAggregateRootChanged);
            _aggregateRootBChanged = _aggregateRootBHandle.RegisterChanged(OnAggregateRootChanged);
            _aggregateRootCChanged = _aggregateRootCHandle.RegisterChanged(OnAggregateRootChanged);
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootA? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootB? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootC? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            var aAvailable = _aggregateRootAHandle.TryGet(out var aggregateRootA);
            var bAvailable = _aggregateRootBHandle.TryGet(out var aggregateRootB);
            var cAvailable = _aggregateRootCHandle.TryGet(out var aggregateRootC);

            if (aAvailable && bAvailable && cAvailable)
            {
                if (!_isAvailable)
                {
                    _isAvailable = true;
                    OnAvailable(aggregateRootA!, aggregateRootB!, aggregateRootC!);
                }

                return;
            }

            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }

            _aggregateRootAChanged?.UnRegister();
            _aggregateRootAChanged = null;
            _aggregateRootBChanged?.UnRegister();
            _aggregateRootBChanged = null;
            _aggregateRootCChanged?.UnRegister();
            _aggregateRootCChanged = null;
        }

        /// <summary>
        /// 所有依赖的 AggregateRoot 可用时调用一次；若 AggregateRoot 失效后恢复，会再次调用。
        /// </summary>
        protected abstract void OnAvailable(
            TAggregateRootA aggregateRootA,
            TAggregateRootB aggregateRootB,
            TAggregateRootC aggregateRootC);

        /// <summary>
        /// 任一依赖的 AggregateRoot 失效时调用一次；组件销毁时若仍处于可用状态，也会调用一次。
        /// </summary>
        protected abstract void OnUnavailable();
    }

    public abstract class MonoVController<TAggregateRootA, TAggregateRootB, TAggregateRootC, TAggregateRootD> :
        MonoVController
        where TAggregateRootA : class, IAggregateRoot
        where TAggregateRootB : class, IAggregateRoot
        where TAggregateRootC : class, IAggregateRoot
        where TAggregateRootD : class, IAggregateRoot
    {
        private IAggregateRootHandle<TAggregateRootA> _aggregateRootAHandle = null!;
        private IAggregateRootHandle<TAggregateRootB> _aggregateRootBHandle = null!;
        private IAggregateRootHandle<TAggregateRootC> _aggregateRootCHandle = null!;
        private IAggregateRootHandle<TAggregateRootD> _aggregateRootDHandle = null!;
        private IUnRegister? _aggregateRootAChanged;
        private IUnRegister? _aggregateRootBChanged;
        private IUnRegister? _aggregateRootCChanged;
        private IUnRegister? _aggregateRootDChanged;
        private bool _isAvailable;

        protected virtual void Awake()
        {
            _aggregateRootAHandle = this.GetAggregateRootHandle<TAggregateRootA>();
            _aggregateRootBHandle = this.GetAggregateRootHandle<TAggregateRootB>();
            _aggregateRootCHandle = this.GetAggregateRootHandle<TAggregateRootC>();
            _aggregateRootDHandle = this.GetAggregateRootHandle<TAggregateRootD>();
            _aggregateRootAChanged = _aggregateRootAHandle.RegisterChanged(OnAggregateRootChanged);
            _aggregateRootBChanged = _aggregateRootBHandle.RegisterChanged(OnAggregateRootChanged);
            _aggregateRootCChanged = _aggregateRootCHandle.RegisterChanged(OnAggregateRootChanged);
            _aggregateRootDChanged = _aggregateRootDHandle.RegisterChanged(OnAggregateRootChanged);
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootA? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootB? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootC? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void OnAggregateRootChanged(TAggregateRootD? aggregateRoot)
        {
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            var aAvailable = _aggregateRootAHandle.TryGet(out var aggregateRootA);
            var bAvailable = _aggregateRootBHandle.TryGet(out var aggregateRootB);
            var cAvailable = _aggregateRootCHandle.TryGet(out var aggregateRootC);
            var dAvailable = _aggregateRootDHandle.TryGet(out var aggregateRootD);

            if (aAvailable && bAvailable && cAvailable && dAvailable)
            {
                if (!_isAvailable)
                {
                    _isAvailable = true;
                    OnAvailable(aggregateRootA!, aggregateRootB!, aggregateRootC!, aggregateRootD!);
                }

                return;
            }

            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_isAvailable)
            {
                _isAvailable = false;
                OnUnavailable();
            }

            _aggregateRootAChanged?.UnRegister();
            _aggregateRootAChanged = null;
            _aggregateRootBChanged?.UnRegister();
            _aggregateRootBChanged = null;
            _aggregateRootCChanged?.UnRegister();
            _aggregateRootCChanged = null;
            _aggregateRootDChanged?.UnRegister();
            _aggregateRootDChanged = null;
        }

        /// <summary>
        /// 所有依赖的 AggregateRoot 可用时调用一次；若 AggregateRoot 失效后恢复，会再次调用。
        /// </summary>
        protected abstract void OnAvailable(
            TAggregateRootA aggregateRootA,
            TAggregateRootB aggregateRootB,
            TAggregateRootC aggregateRootC,
            TAggregateRootD aggregateRootD);

        /// <summary>
        /// 任一依赖的 AggregateRoot 失效时调用一次；组件销毁时若仍处于可用状态，也会调用一次。
        /// </summary>
        protected abstract void OnUnavailable();
    }
}
