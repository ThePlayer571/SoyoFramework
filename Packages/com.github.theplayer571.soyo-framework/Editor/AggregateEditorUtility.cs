using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SoyoFramework.Editor
{
    internal static class AggregateEditorUtility
    {
        internal const string LifecycleCheckPreference = "SoyoFramework.Aggregate.LifecycleCheck";

        private static readonly HashSet<string> FrameworkAssemblyPrefixes = new(StringComparer.OrdinalIgnoreCase)
        {
            "SoyoFramework",
            "System",
            "Unity",
            "UnityEditor",
            "mscorlib",
            "netstandard",
            "Microsoft"
        };

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            AggregateHigherThan.LifecycleCheckEnabled =
                EditorPrefs.GetBool(LifecycleCheckPreference, true);
        }

        internal static bool LifecycleCheckEnabled
        {
            get => AggregateHigherThan.LifecycleCheckEnabled;
            set
            {
                AggregateHigherThan.LifecycleCheckEnabled = value;
                EditorPrefs.SetBool(LifecycleCheckPreference, value);
            }
        }

        internal static Assembly[] GetSmartAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !IsExcludedAssembly(assembly.GetName().Name ?? string.Empty))
                .Where(assembly => assembly.GetReferencedAssemblies()
                    .Any(reference => string.Equals(reference.Name, "SoyoFramework", StringComparison.OrdinalIgnoreCase)))
                .OrderBy(assembly => assembly.GetName().Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        internal static Assembly[] GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .OrderBy(assembly => assembly.GetName().Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        internal static bool IsExcludedAssembly(string assemblyName)
        {
            return FrameworkAssemblyPrefixes.Any(prefix =>
                assemblyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        internal static Type[] GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type != null).Cast<Type>().ToArray();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }
    }
}
