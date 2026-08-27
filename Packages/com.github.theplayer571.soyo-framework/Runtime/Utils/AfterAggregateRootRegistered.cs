namespace SoyoFramework.Utils
{
    public class AfterAggregateRootRegistered<T> where T : class, IAggregateRoot
    {
        public T AggregateRoot { get; }

        public AfterAggregateRootRegistered(T aggregateRoot)
        {
            AggregateRoot = aggregateRoot;
        }
    }
}
