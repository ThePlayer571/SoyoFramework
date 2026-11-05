using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoyoFramework.Editor
{
    internal static class AggregateEditorUtility
    {
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
