using SoyoFramework.Framework.Runtime.Core.Domain;

namespace SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar
{
    public abstract class DefaultDomainRoot<TRoot> : DomainRoot<TRoot>, IDefaultDomainRoot
        where TRoot : IDomainRoot
    {
        protected DefaultDomainRoot(TRoot root) : base(root)
        {
        }
    }
}