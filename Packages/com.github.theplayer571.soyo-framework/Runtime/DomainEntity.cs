using System;
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
            get => _root ??
                   throw new InvalidOperationException("尝试在 MonoDomainEntity 初始化前访问 Root。请先调用 Root.setter 来设置 Root");
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

        private TRoot? _root;

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }
}