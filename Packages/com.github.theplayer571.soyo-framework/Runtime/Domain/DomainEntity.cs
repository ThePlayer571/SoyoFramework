using UnityEngine;

namespace SoyoFramework.Domain
{
    public abstract class DomainEntity<TRoot> : IDomainRule
        where TRoot : IDomainRoot
    {
        protected DomainEntity(TRoot root)
        {
            Root = root;
        }

        protected TRoot Root { get; }

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }

    public abstract class MonoDomainEntity<TRoot> : MonoBehaviour, IDomainRule
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

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }
}