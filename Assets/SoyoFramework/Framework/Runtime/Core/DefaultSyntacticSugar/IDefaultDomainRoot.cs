namespace SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar
{
    public interface IDefaultDomainRoot : IDomainRoot
    {
        IArchitecture ICanRelyOnArchitecture.RelyingArchitecture => DefaultArchitecture.Instance;
    }
}