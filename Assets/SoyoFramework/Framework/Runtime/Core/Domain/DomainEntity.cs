namespace SoyoFramework.Framework.Runtime.Core.Domain
{
    public interface IDomainEntity
    {
    }

    public abstract class DomainEntity<TRoot> : IDomainEntity
        where TRoot : IDomainRoot
    {
        protected DomainEntity(TRoot root)
        {
            Root = root;
        }

        protected TRoot Root { get; private set; }
    }
}