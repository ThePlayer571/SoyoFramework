using SoyoFramework.Framework.Runtime.Core.Domain;

namespace SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar
{
    public abstract class DefaultDomainRoot : DomainRoot
    {
        protected DefaultDomainRoot() : base(DefaultArchitecture.Instance)
        {
        }
    }
}