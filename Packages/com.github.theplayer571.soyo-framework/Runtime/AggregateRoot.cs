namespace SoyoFramework
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        protected AggregateRoot()
        {
        }

        IAggregateRoot IAggregateMember.AggregateRoot => this;

        void IAggregateRoot.OnUnregister()
        {
            OnUnregister();
        }

        protected abstract void OnUnregister();
    }
}
