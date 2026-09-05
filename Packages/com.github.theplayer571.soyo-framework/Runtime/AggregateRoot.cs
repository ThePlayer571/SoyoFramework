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
                $"{nameof(GlobalArchitecture)} 未初始化。你应该调用对应 Architecture 的 Init 方法来初始化框架。\n" +
                $"如果你不打算使用 {nameof(GlobalArchitecture)}，请重写 {nameof(RelyingArchitecture)} 并返回正确的 {nameof(IArchitecture)} 实例。");

        IAggregateRoot IAggregateMember.AggregateRoot => this;

        void IAggregateRoot.Deinit()
        {
            OnUnregistered();
        }

        protected abstract void OnUnregistered();
    }
}
