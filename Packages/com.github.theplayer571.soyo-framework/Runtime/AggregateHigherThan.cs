using System;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endif

namespace SoyoFramework
{
    public static class AggregateHigherThan
    {
#if UNITY_EDITOR
        internal static bool LifecycleCheckEnabled { get; set; } = true;
#endif

        /// <summary>
        /// 检查所属聚合能否管理指定聚合 key。仅在 Editor 中执行检查；Player 中始终返回 true。
        /// </summary>
        public static bool ValidateLifecycle<T>(IAggregateMember owner)
            where T : class, IAggregateRoot
            => ValidateLifecycle(owner, typeof(T));

        /// <summary>
        /// 检查所属聚合能否管理指定聚合 key。检查失败不阻止注册或注销。
        /// </summary>
        public static bool ValidateLifecycle(IAggregateMember owner, Type targetKey)
        {
#if UNITY_EDITOR
            if (!LifecycleCheckEnabled)
            {
                return true;
            }

            return IsHigherThan(owner.AggregateRoot.GetType(), targetKey);
#else
            return true;
#endif
        }

#if UNITY_EDITOR
        internal static bool IsHigherThan(Type higherType, Type lowerType)
        {
            if (higherType == lowerType)
            {
                return false;
            }

            var visited = new HashSet<Type>();
            var pending = new Queue<Type>();
            pending.Enqueue(higherType);

            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                if (!visited.Add(current))
                {
                    continue;
                }

                foreach (var next in GetEffectiveLowerTypes(current))
                {
                    if (next == lowerType)
                    {
                        return true;
                    }

                    pending.Enqueue(next);
                }
            }

            return false;
        }

        internal static IReadOnlyList<Type> GetEffectiveLowerTypes(Type sourceType)
        {
            var result = new List<Type>();
            foreach (var declarationType in GetDeclarationTypes(sourceType))
            {
                var attribute = declarationType.GetCustomAttributes(typeof(HigherThanAttribute), false)
                    .OfType<HigherThanAttribute>()
                    .FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

                foreach (var lowerType in attribute.LowerTypes)
                {
                    if (!result.Contains(lowerType))
                    {
                        result.Add(lowerType);
                    }
                }
            }

            return result;
        }

        internal static IEnumerable<Type> GetDeclarationTypes(Type sourceType)
        {
            var result = new List<Type>();
            for (var current = sourceType; current != null; current = current.BaseType)
            {
                result.Add(current);
            }

            result.AddRange(sourceType.GetInterfaces());
            return result.Distinct();
        }
#endif
    }
}
