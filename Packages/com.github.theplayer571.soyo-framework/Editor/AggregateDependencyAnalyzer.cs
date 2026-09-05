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
            var selectedAssemblies = assemblies.Distinct().ToArray();
            var selectedAssemblySet = new HashSet<Assembly>(selectedAssemblies);
            var selectedTypes = selectedAssemblies
                .SelectMany(AggregateEditorUtility.GetAssemblyTypes)
                .Distinct()
                .ToArray();

            var directEdges = new Dictionary<Type, List<Type>>();
            var nodes = new HashSet<Type>();

            foreach (var sourceType in selectedTypes)
            {
                var attributes = sourceType.GetCustomAttributes(typeof(HigherThanAttribute), false)
                    .OfType<HigherThanAttribute>()
                    .ToArray();
                if (attributes.Length == 0)
                {
                    continue;
                }

                nodes.Add(sourceType);
                if (!typeof(IAggregateRoot).IsAssignableFrom(sourceType))
                {
                    result.Errors.Add($"{sourceType.Name} 上的 HigherThan 来源类型不是 IAggregateRoot。");
                }

                var lowerTypes = attributes[0].LowerTypes;
                if (lowerTypes.Length == 0)
                {
                    result.Errors.Add($"{sourceType.Name} 的 HigherThan 目标列表不能为空。");
                }

                var edgeTargets = new List<Type>();
                foreach (var lowerType in lowerTypes)
                {
                    if (lowerType == null)
                    {
                        result.Errors.Add($"{sourceType.Name} 的 HigherThan 包含空目标类型。");
                        continue;
                    }

                    if (!typeof(IAggregateRoot).IsAssignableFrom(lowerType))
                    {
                        result.Errors.Add(
                            $"{sourceType.Name} 的 HigherThan 目标 {lowerType.Name} 不是 IAggregateRoot。");
                    }

                    if (lowerType.ContainsGenericParameters)
                    {
                        result.Errors.Add(
                            $"{sourceType.Name} 的 HigherThan 目标 {lowerType.Name} 不能是开放泛型类型。");
                    }

                    if (lowerType == sourceType)
                    {
                        result.Errors.Add($"{sourceType.Name} 不能 HigherThan 自己。");
                    }

                    if (edgeTargets.Contains(lowerType))
                    {
                        result.Errors.Add($"{sourceType.Name} 重复声明 HigherThan {lowerType.Name}。");
                    }
                    else
                    {
                        edgeTargets.Add(lowerType);
                        nodes.Add(lowerType);
                    }

                    if (!selectedAssemblySet.Contains(lowerType.Assembly))
                    {
                        result.Errors.Add(
                            $"缺少程序集：{sourceType.Name} 的 HigherThan 目标 {lowerType.Name} 所在程序集未被选中。");
                    }
                }

                directEdges[sourceType] = edgeTargets;
            }

            var effectiveEdges = new Dictionary<Type, IReadOnlyList<Type>>();
            var pendingNodes = new Queue<Type>(nodes);
            while (pendingNodes.Count > 0)
            {
                var node = pendingNodes.Dequeue();
                if (effectiveEdges.ContainsKey(node))
                {
                    continue;
                }

                // 指定程序集模式下，目标类型可以作为图节点出现并报告缺失，
                // 但不能因为它出现于边终点就继续递归读取其所在程序集的规则。
                var targets = selectedAssemblySet.Contains(node.Assembly)
                    ? AggregateHigherThan.GetEffectiveLowerTypes(node)
                        .Distinct()
                        .ToArray()
                    : Array.Empty<Type>();
                effectiveEdges[node] = targets;
                foreach (var target in targets)
                {
                    if (!selectedAssemblySet.Contains(target.Assembly))
                    {
                        result.Errors.Add(
                            $"缺少程序集：{node.Name} 的 HigherThan 目标 {target.Name} 所在程序集未被选中。");
                    }

                    if (nodes.Add(target))
                    {
                        pendingNodes.Enqueue(target);
                    }
                }
            }

            foreach (var cycle in FindCycles(nodes, effectiveEdges))
            {
                result.Errors.Add($"发现环形 HigherThan 依赖：{string.Join(" -> ", cycle.Select(type => type.Name))}");
            }

            if (!result.HasErrors)
            {
                result.Mermaid = BuildMermaid(directEdges);
            }

            return result;
        }

        private static string BuildMermaid(Dictionary<Type, List<Type>> directEdges)
        {
            var builder = new StringBuilder("graph TD");
            foreach (var source in directEdges.Keys.OrderBy(type => type.Name, StringComparer.Ordinal))
            {
                foreach (var target in directEdges[source].OrderBy(type => type.Name, StringComparer.Ordinal))
                {
                    builder.AppendLine();
                    builder.Append("    ")
                        .Append(source.Name)
                        .Append(" --> ")
                        .Append(target.Name);
                }
            }

            return builder.ToString();
        }

        private static IEnumerable<IReadOnlyList<Type>> FindCycles(
            HashSet<Type> nodes,
            Dictionary<Type, IReadOnlyList<Type>> edges)
        {
            var cycles = new Dictionary<string, IReadOnlyList<Type>>();
            foreach (var start in nodes)
            {
                var path = new List<Type> { start };
                var visited = new HashSet<Type> { start };
                FindCyclesFrom(start, start, edges, path, visited, cycles);
            }

            return cycles.Values.OrderBy(cycle => string.Join("|", cycle.Select(type => type.Name)));
        }

        private static void FindCyclesFrom(
            Type start,
            Type current,
            Dictionary<Type, IReadOnlyList<Type>> edges,
            List<Type> path,
            HashSet<Type> visited,
            Dictionary<string, IReadOnlyList<Type>> cycles)
        {
            if (!edges.TryGetValue(current, out var targets))
            {
                return;
            }

            foreach (var target in targets)
            {
                if (target == start)
                {
                    var cycle = path.ToArray();
                    var key = CanonicalCycleKey(cycle);
                    if (!cycles.ContainsKey(key))
                    {
                        cycles.Add(key, cycle.Append(start).ToArray());
                    }
                }
                else if (!visited.Contains(target))
                {
                    visited.Add(target);
                    path.Add(target);
                    FindCyclesFrom(start, target, edges, path, visited, cycles);
                    path.RemoveAt(path.Count - 1);
                    visited.Remove(target);
                }
            }
        }

        private static string CanonicalCycleKey(IReadOnlyList<Type> cycle)
        {
            // cycle 是不含重复首节点的搜索路径；只对有向环做旋转归一化，
            // 避免同一环从不同起点被重复输出。
            var names = cycle
                .Select(type => type.AssemblyQualifiedName ?? type.FullName ?? type.Name)
                .ToArray();
            var rotations = Enumerable.Range(0, names.Length)
                .Select(offset => string.Join("|", Enumerable.Range(0, names.Length)
                    .Select(index => names[(offset + index) % names.Length])));
            return rotations.OrderBy(value => value, StringComparer.Ordinal).First();
        }
    }
}
