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

        /// <summary>
        /// 聚合根注销回调。回调时已被移出底层容器（无法被 <see cref="IArchitecture.GetAggregateRoot"/> 查找）。
        /// </summary>
        protected abstract void OnUnregister();
    }
}