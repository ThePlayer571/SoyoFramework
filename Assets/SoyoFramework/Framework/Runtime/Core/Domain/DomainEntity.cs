using UnityEngine;

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

        protected TRoot Root { get; }
    }

    public abstract class MonoDomainEntity<TRoot> : MonoBehaviour, IDomainEntity
        where TRoot : IDomainRoot
    {
        protected TRoot Root
        {
            get
            {
                _root ??= GetRoot();
                return _root;
            }
        }

        private TRoot _root;

        protected abstract TRoot GetRoot();
    }
}