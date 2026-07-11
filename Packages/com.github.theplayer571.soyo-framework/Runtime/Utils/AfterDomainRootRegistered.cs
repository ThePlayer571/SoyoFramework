namespace SoyoFramework.Utils
{
    public class AfterDomainRootRegistered<T> where T : class, IDomainRoot
    {
        public T DomainRoot { get; }

        public AfterDomainRootRegistered(T domainRoot)
        {
            DomainRoot = domainRoot;
        }
    }
}