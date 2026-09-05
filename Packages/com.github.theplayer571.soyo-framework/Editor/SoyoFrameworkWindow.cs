using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Editor
{
    public sealed class SoyoFrameworkWindow : EditorWindow
    {
        private const string AssemblyModePreference = "SoyoFramework.Aggregate.AssemblyMode";
        private const string ExplicitAssembliesPreference = "SoyoFramework.Aggregate.ExplicitAssemblies";

        private enum AssemblyMode
        {
            Smart,
            Explicit
        }

        private AssemblyMode _assemblyMode;
        private readonly List<string> _explicitAssemblyNames = new();
        private string _result = string.Empty;
        private Vector2 _resultScroll;

        [MenuItem("Window/Soyo Framework")]
        private static void Open()
        {
            GetWindow<SoyoFrameworkWindow>("Soyo Framework");
        }

        private void OnEnable()
        {
            _assemblyMode = (AssemblyMode)EditorPrefs.GetInt(AssemblyModePreference, (int)AssemblyMode.Smart);
            _explicitAssemblyNames.Clear();
            var saved = EditorPrefs.GetString(ExplicitAssembliesPreference, string.Empty);
            _explicitAssemblyNames.AddRange(saved.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Aggregate相关", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            var lifecycleCheck = EditorGUILayout.ToggleLeft("生命周期检查", AggregateEditorUtility.LifecycleCheckEnabled,
                GUILayout.Width(120));
            if (lifecycleCheck != AggregateEditorUtility.LifecycleCheckEnabled)
            {
                AggregateEditorUtility.LifecycleCheckEnabled = lifecycleCheck;
            }

            EditorGUILayout.LabelField("Register/Unregister 非 HigherThan 下位聚合时记录错误", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("程序集范围", EditorStyles.boldLabel);
            var mode = (AssemblyMode)GUILayout.Toolbar((int)_assemblyMode, new[] { "智能选择", "指定程序集" });
            if (mode != _assemblyMode)
            {
                _assemblyMode = mode;
                EditorPrefs.SetInt(AssemblyModePreference, (int)_assemblyMode);
            }

            if (_assemblyMode == AssemblyMode.Explicit)
            {
                DrawExplicitAssemblyList();
            }

            EditorGUILayout.Space(8);
            if (GUILayout.Button("分析 Aggregate 依赖"))
            {
                Analyze();
            }

            EditorGUILayout.Space(6);
            _resultScroll = EditorGUILayout.BeginScrollView(_resultScroll, GUI.skin.box);
            EditorGUILayout.TextArea(_result, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void DrawExplicitAssemblyList()
        {
            var loadedAssemblies = AggregateEditorUtility.GetLoadedAssemblies();
            var options = loadedAssemblies.Select(assembly => assembly.GetName().Name).ToArray();

            for (var index = 0; index < _explicitAssemblyNames.Count; index++)
            {
                EditorGUILayout.BeginHorizontal();
                var currentName = _explicitAssemblyNames[index];
                var currentIndex = Array.IndexOf(options, currentName);
                var popupOptions = options;
                var missingSelection = currentIndex < 0;
                if (missingSelection)
                {
                    popupOptions = new[] { $"<未加载> {currentName}" }.Concat(options).ToArray();
                }

                var popupIndex = missingSelection ? 0 : currentIndex;
                var selectedIndex = EditorGUILayout.Popup(popupIndex, popupOptions);
                if (popupOptions.Length > 0 && selectedIndex >= 0 && selectedIndex != popupIndex)
                {
                    var optionIndex = missingSelection ? selectedIndex - 1 : selectedIndex;
                    if (optionIndex >= 0 && optionIndex < options.Length)
                    {
                        _explicitAssemblyNames[index] = options[optionIndex];
                    }
                }

                if (GUILayout.Button("-", GUILayout.Width(24)))
                {
                    _explicitAssemblyNames.RemoveAt(index);
                    index--;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("添加程序集"))
            {
                var candidate = options.FirstOrDefault(name => !_explicitAssemblyNames.Contains(name));
                if (!string.IsNullOrEmpty(candidate))
                {
                    _explicitAssemblyNames.Add(candidate);
                }
            }

            EditorPrefs.SetString(ExplicitAssembliesPreference, string.Join("\n", _explicitAssemblyNames));
        }

        private void Analyze()
        {
            Assembly[] assemblies;
            var errors = new List<string>();
            if (_assemblyMode == AssemblyMode.Smart)
            {
                assemblies = AggregateEditorUtility.GetSmartAssemblies();
            }
            else
            {
                var loadedByName = AggregateEditorUtility.GetLoadedAssemblies()
                    .ToDictionary(assembly => assembly.GetName().Name ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                var selected = new List<Assembly>();
                foreach (var name in _explicitAssemblyNames.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (loadedByName.TryGetValue(name, out var assembly))
                    {
                        selected.Add(assembly);
                    }
                    else
                    {
                        errors.Add($"指定的程序集未加载：{name}");
                    }
                }

                assemblies = selected.ToArray();
            }

            var result = AggregateDependencyAnalyzer.Analyze(assemblies);
            errors.AddRange(result.Errors);
            _result = errors.Count == 0 ? result.Mermaid : string.Join("\n", errors.Distinct());
        }
    }
}
