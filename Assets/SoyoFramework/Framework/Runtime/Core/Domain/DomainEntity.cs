namespace SoyoFramework.Framework.Runtime.Core.Domain
{
    public interface IDomainEntity<TRoot> where TRoot : IDomainRoot
    {
    }

    public abstract class DomainEntity<TRoot> : IDomainEntity<TRoot>
        where TRoot : IDomainRoot
    {
        protected DomainEntity(TRoot root)
        {
            Root = root;
        }

        protected TRoot Root { get; private set; }
    }
}