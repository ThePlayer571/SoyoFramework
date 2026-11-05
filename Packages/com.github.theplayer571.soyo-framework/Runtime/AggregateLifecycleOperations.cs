using SoyoFramework.Utils.LogKit;

namespace SoyoFramework
{
    /// <summary>
    /// 聚合成员的生命周期操作。Editor 中检查 HigherThan 关系，失败时记录错误但仍执行操作。
    /// </summary>
    public static class AggregateLifecycleOperations
    {
        /// <summary>
        /// 以 T 为 key 注册聚合根。
        /// </summary>
        public static void Register<T>(IAggregateMember owner, T aggregateRoot)
            where T : class, IAggregateRoot
        {
#if UNITY_EDITOR
            if (!AggregateHigherThan.ValidateLifecycle<T>(owner))
            {
                $"Aggregate {owner.AggregateRoot.GetType().Name} 不能注册 {typeof(T).Name}：目标不是当前聚合的 HigherThan 下位聚合。"
                    .LogError();
            }
#endif
            Architecture.Instance.RegisterAggregateRoot(aggregateRoot);
        }

        /// <summary>
        /// 以 T 为 key 注销聚合根。
        /// </summary>
        public static void Unregister<T>(IAggregateMember owner)
            where T : class, IAggregateRoot
        {
#if UNITY_EDITOR
            if (!AggregateHigherThan.ValidateLifecycle<T>(owner))
            {
                $"Aggregate {owner.AggregateRoot.GetType().Name} 不能注销 {typeof(T).Name}：目标不是当前聚合的 HigherThan 下位聚合。"
                    .LogError();
            }
#endif
            Architecture.Instance.UnregisterAggregateRoot<T>();
        }
    }
}
