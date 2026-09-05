using System;
using SoyoFramework.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework
{
    public abstract class AggregateMember : IAggregateMember
    {
        protected AggregateMember(IAggregateRoot root)
        {
            Root = root;
        }

        protected IAggregateRoot Root { get; }

        IAggregateRoot IAggregateMember.AggregateRoot => Root;

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }

    public abstract class AggregateMember<TRoot> : IAggregateMember
        where TRoot : IAggregateRoot
    {
        protected AggregateMember(TRoot root)
        {
            Root = root;
        }

        protected TRoot Root { get; }

        IAggregateRoot IAggregateMember.AggregateRoot => Root;

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }

    public abstract class MonoAggregateMember<TRoot> : MonoBehaviour, IAggregateMember
        where TRoot : IAggregateRoot
    {
        protected TRoot Root
        {
            get => _root ??
                   throw new InvalidOperationException($"尝试在 {nameof(MonoAggregateMember<TRoot>)} 初始化前访问 {nameof(Root)}。请先调用 {nameof(Root)}.setter 来设置 {nameof(Root)}");
            set
            {
                if (_root != null)
                {
                    $"{nameof(Root)} 已经被设置过了，不能再次设置: {_root} -> {value}".LogError();
                }
                else
                {
                    _root = value;
                }
            }
        }

        private TRoot? _root;

        IAggregateRoot IAggregateMember.AggregateRoot => Root;

        public IArchitecture RelyingArchitecture => Root.RelyingArchitecture;
    }
}
