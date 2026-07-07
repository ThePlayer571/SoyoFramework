namespace SoyoFramework.Framework.Runtime.Core.Domain
{
    public abstract class DomainRoot : IDomainRoot
    {
        protected DomainRoot(IArchitecture arch)
        {
            RelyingArchitecture = arch;
        }

        public IArchitecture RelyingArchitecture { get; }

        void IDomainRoot.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnDeinit();
    }
}