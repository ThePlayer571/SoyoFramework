using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoyoFramework.Editor
{
    internal sealed class AggregateDependencyAnalysisResult
    {
        internal readonly List<string> Errors = new();
        internal string Mermaid = string.Empty;
        internal bool HasErrors => Errors.Count != 0;
    }

    internal static class AggregateDependencyAnalyzer
    {
        internal static AggregateDependencyAnalysisResult Analyze(IEnumerable<Assembly> assemblies)
        {
            var result = new AggregateDependencyAnalysisResult();
            var selected = new HashSet<Assembly>(assemblies);
            var types = selected.SelectMany(AggregateEditorUtility.GetAssemblyTypes).Distinct().ToArray();
            var edges = new List<(Type Source, Type Target, AggregateRelationKind Kind)>();
            var requires = new Dictionary<Type, IReadOnlyList<Type>>();

            foreach (var type in types)
            {
                var direct = AggregateRelations.GetDeclarations(type).ToArray();
                if (direct.Length > 0 && !typeof(IAggregateRoot).IsAssignableFrom(type))
                {
                    result.Errors.Add($"{type.Name} 声明了聚合关系，但不是 IAggregateRoot。");
                }

                foreach (var edge in direct)
                {
                    if (edge.Target == null || !typeof(IAggregateRoot).IsAssignableFrom(edge.Target) ||
                        edge.Target.ContainsGenericParameters || edge.Target == type)
                    {
                        result.Errors.Add($"{type.Name} 的 {edge.Kind} 目标无效：{edge.Target?.Name ?? "null"}。");
                        continue;
                    }

                    if (!selected.Contains(edge.Target.Assembly))
                    {
                        result.Errors.Add($"缺少程序集：{type.Name} 的 {edge.Kind} 目标 {edge.Target.Name} 所在程序集未被选中。");
                    }

                    edges.Add(edge);
                }

                if (!typeof(IAggregateRoot).IsAssignableFrom(type))
                {
                    continue;
                }

                if (!AggregateRelations.TryGet(type, out var effective, out var error))
                {
                    result.Errors.Add(error);
                    continue;
                }

                foreach (var target in effective.Keys.Where(target => !selected.Contains(target.Assembly)))
                {
                    result.Errors.Add($"缺少程序集：{type.Name} 的目标 {target.Name} 所在程序集未被选中。");
                }

                requires[type] = effective.Where(item => item.Value == AggregateRelationKind.Requires)
                    .Select(item => item.Key).Where(target => selected.Contains(target.Assembly)).ToArray();
            }

            foreach (var cycle in FindCycles(requires))
            {
                result.Errors.Add($"发现环形 Requires 依赖：{string.Join(" -> ", cycle.Select(type => type.Name))}");
            }

            result.Mermaid = BuildMermaid(edges);
            return result;
        }

        private static string BuildMermaid(IEnumerable<(Type Source, Type Target, AggregateRelationKind Kind)> edges)
        {
            var builder = new StringBuilder("graph TD");
            foreach (var edge in edges.Distinct().OrderBy(edge => edge.Source.Name, StringComparer.Ordinal)
                         .ThenBy(edge => edge.Target.Name, StringComparer.Ordinal))
            {
                builder.AppendLine();
                builder.Append("    ").Append(edge.Source.Name).Append(" -- ").Append(edge.Kind)
                    .Append(" --> ").Append(edge.Target.Name);
            }

            return builder.ToString();
        }

        private static IEnumerable<IReadOnlyList<Type>> FindCycles(Dictionary<Type, IReadOnlyList<Type>> edges)
        {
            var cycles = new Dictionary<string, IReadOnlyList<Type>>();
            foreach (var start in edges.Keys)
            {
                Walk(start, start, edges, new List<Type> { start }, new HashSet<Type> { start }, cycles);
            }

            return cycles.Values;
        }

        private static void Walk(Type start, Type current, Dictionary<Type, IReadOnlyList<Type>> edges,
            List<Type> path, HashSet<Type> visited, Dictionary<string, IReadOnlyList<Type>> cycles)
        {
            if (!edges.TryGetValue(current, out var targets)) return;
            foreach (var target in targets)
            {
                if (target == start)
                {
                    var names = path.Select(type => type.AssemblyQualifiedName ?? type.FullName ?? type.Name).ToArray();
                    var key = Enumerable.Range(0, names.Length)
                        .Select(offset => string.Join("|", Enumerable.Range(0, names.Length)
                            .Select(index => names[(offset + index) % names.Length])))
                        .OrderBy(value => value, StringComparer.Ordinal).First();
                    if (!cycles.ContainsKey(key)) cycles[key] = path.Concat(new[] { start }).ToArray();
                }
                else if (visited.Add(target))
                {
                    path.Add(target);
                    Walk(start, target, edges, path, visited, cycles);
                    path.RemoveAt(path.Count - 1);
                    visited.Remove(target);
                }
            }
        }
    }
}
