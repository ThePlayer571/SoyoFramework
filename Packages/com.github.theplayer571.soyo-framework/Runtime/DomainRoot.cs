using System;

namespace SoyoFramework
{
    public abstract class DomainRoot : IDomainRoot
    {
        protected DomainRoot()
        {
        }

        public virtual IArchitecture RelyingArchitecture
            => GlobalArchitecture.Instance ??
               throw new InvalidOperationException("GlobalArchitecture 未初始化。你应该调用 Architecture.Init() 来初始化框架");

        void IDomainRoot.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnDeinit();
    }
}