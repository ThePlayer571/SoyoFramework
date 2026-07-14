using SoyoFramework.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework
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
            get => _root;
            set
            {
                if (_root != null)
                {
                    $"Root 已经被设置过了，不能再次设置: {_root} -> {value}".LogError();
                }
                else
                {
                    _root = value;
                }
            }
        }

        private TRoot _root;

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }
}