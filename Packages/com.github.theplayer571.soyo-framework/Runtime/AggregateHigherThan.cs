#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoyoFramework
{
    internal static class AggregateHigherThan
    {
        internal static bool LifecycleCheckEnabled { get; set; } = true;

        internal static bool ValidateLifecycle(
            IAggregateRoot aggregateRoot,
            Type targetType)
        {
            if (!LifecycleCheckEnabled)
            {
                return true;
            }

            var rootType = aggregateRoot.GetType();
            if (IsHigherThan(rootType, targetType))
            {
                return true;
            }

            return false;
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
