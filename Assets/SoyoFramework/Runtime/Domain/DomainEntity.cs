using UnityEngine;

namespace SoyoFramework.Runtime.Domain
{
    public abstract class DomainEntity<TRoot> where TRoot : IDomainRoot
    {
        protected DomainEntity(TRoot root)
        {
            Root = root;
        }

        protected TRoot Root { get; }
    }

    public abstract class MonoDomainEntity<TRoot> : MonoBehaviour
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