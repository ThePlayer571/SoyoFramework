namespace SoyoFramework
{
    public abstract class DomainRoot : IDomainRoot
    {
        protected DomainRoot()
        {
        }

        public virtual IArchitecture RelyingArchitecture => GlobalArchitecture.Instance;

        void IDomainRoot.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnDeinit();
    }
}