#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SoyoFramework
{
    internal enum AggregateLifecycleOperation
    {
        Register,
        Unregister
    }

    internal static class AggregateHigherThan
    {
        internal static bool LifecycleCheckEnabled { get; set; } = true;

        internal static void ValidateLifecycle(
            IAggregateRoot aggregateRoot,
            Type targetType,
            AggregateLifecycleOperation operation)
        {
            if (!LifecycleCheckEnabled)
            {
                return;
            }

            var rootType = aggregateRoot.GetType();
            if (IsHigherThan(rootType, targetType))
            {
                return;
            }

            var operationName = operation == AggregateLifecycleOperation.Register ? "注册" : "注销";
            Debug.LogError(
                $"Aggregate {rootType.Name} 不能{operationName} {targetType.Name}：目标不是当前聚合的 HigherThan 下位聚合。");
        }

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
    }
}
#endif
