namespace SoyoFramework.Framework.Runtime.Core.Domain
{
    public abstract class DomainRoot<TRoot> : DomainEntity<TRoot>, IDomainRoot
        where TRoot : IDomainRoot
    {
        protected DomainRoot(TRoot root) : base(root)
        {
        }

        public abstract IArchitecture RelyingArchitecture { get; }

        void IDomainRoot.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnDeinit();
    }
}