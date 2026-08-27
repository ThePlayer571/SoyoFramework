using System;

namespace SoyoFramework
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        protected AggregateRoot()
        {
        }

        public virtual IArchitecture RelyingArchitecture
            => GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                "GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架"
                + $"如果你不打算使用 GlobalArchitecture，请重写 {nameof(RelyingArchitecture)} 并返回正确的 IArchitecture 实例。");

        void IAggregateRoot.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnDeinit();
    }
}
