namespace SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar
{
    public interface IDefaultMonoVController : IMonoVController
    {
        IArchitecture ICanRelyOnArchitecture.RelyingArchitecture => DefaultArchitecture.Instance;
    }
}