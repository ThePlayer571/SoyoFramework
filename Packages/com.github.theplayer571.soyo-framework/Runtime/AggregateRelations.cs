using System;
using System.Collections.Generic;
using System.Linq;

namespace SoyoFramework
{
    internal enum AggregateRelationKind { Observes, Requires, Manages }

    internal static class AggregateRelations
    {
        private static readonly Dictionary<Type, IReadOnlyDictionary<Type, AggregateRelationKind>> Cache = new();

        internal static IEnumerable<(Type Source, Type Target, AggregateRelationKind Kind)> GetDeclarations(Type type)
        {
            foreach (var attribute in type.GetCustomAttributes(typeof(AggregateRelationAttribute), false)
                         .Cast<AggregateRelationAttribute>())
            {
                var kind = attribute switch
                {
                    ObservesAttribute => AggregateRelationKind.Observes,
                    RequiresAttribute => AggregateRelationKind.Requires,
                    ManagesAttribute => AggregateRelationKind.Manages,
                    _ => throw new InvalidOperationException("未知的聚合关系特性。")
                };
                foreach (var target in attribute.TargetTypes)
                {
                    yield return (type, target, kind);
                }
            }
        }

        internal static IEnumerable<Type> GetDeclarationTypes(Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                yield return current;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                yield return @interface;
            }
        }

        internal static bool TryGet(Type type, out IReadOnlyDictionary<Type, AggregateRelationKind> relations,
            out string error)
        {
            if (Cache.TryGetValue(type, out var cached))
            {
                relations = cached;
                error = string.Empty;
                return true;
            }

            var result = new Dictionary<Type, AggregateRelationKind>();
            foreach (var declarationType in GetDeclarationTypes(type))
            {
                var attributes = declarationType.GetCustomAttributes(typeof(AggregateRelationAttribute), false)
                    .Cast<AggregateRelationAttribute>().ToArray();
                foreach (var attribute in attributes)
                {
                    if (attribute.TargetTypes.Length == 0)
                    {
                        relations = result;
                        error = $"{declarationType.Name} 的关系目标列表不能为空。";
                        return false;
                    }

                    foreach (var (_, target, kind) in GetAttributeDeclarations(declarationType, attribute))
                    {
                        if (target == null || !typeof(IAggregateRoot).IsAssignableFrom(target) ||
                            target.ContainsGenericParameters || target == type)
                        {
                            relations = result;
                            error = $"{type.Name} 声明了无效的聚合关系目标 {target?.Name ?? "null"}。";
                            return false;
                        }

                        if (result.TryGetValue(target, out var existing) && existing != kind)
                        {
                            relations = result;
                            error = $"{type.Name} 对 {target.Name} 同时声明了 {existing} 和 {kind}。";
                            return false;
                        }

                        result[target] = kind;
                    }
                }
            }

            relations = result;
            Cache[type] = result;
            error = string.Empty;
            return true;
        }

        private static IEnumerable<(Type Source, Type Target, AggregateRelationKind Kind)> GetAttributeDeclarations(
            Type source, AggregateRelationAttribute attribute)
        {
            var kind = attribute switch
            {
                ObservesAttribute => AggregateRelationKind.Observes,
                RequiresAttribute => AggregateRelationKind.Requires,
                ManagesAttribute => AggregateRelationKind.Manages,
                _ => throw new InvalidOperationException("未知的聚合关系特性。")
            };
            return attribute.TargetTypes.Select(target => (source, target, kind));
        }
    }
}
