using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SoyoFramework.Framework.Runtime.Core.CommandProfiler;

namespace SoyoFramework.Framework.Editor.CommandProfiler
{
    public class CommandAnalyzerWindow : EditorWindow
    {
        private enum SortColumn
        {
            TypeName,
            TotalCount,
            PeakPerSecond
        }

        private SortColumn _sortColumn = SortColumn.TotalCount;
        private bool _sortDescending = true;
        private Vector2 _scrollPosition;

        // 缓存排序后的数据
        private readonly List<KeyValuePair<Type, CommandStat>> _sortedStats = new();
        private double _lastRefreshTime;
        private const double RefreshInterval = 0.5;

        // 用于在 Layout 事件中缓存数据，确保 Layout 和 Repaint 使用相同数据
        private readonly List<KeyValuePair<Type, CommandStat>> _displayStats = new();

        // Settings 引用
        private CommandAnalyzerSettings Settings => CommandAnalyzerSettings.instance;

        [MenuItem("SoyoFramework/Command Analyzer Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<CommandAnalyzerWindow>();
            window.titleContent = new GUIContent("Command Analyzer");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying && CommandSendHook.Enabled)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            // 仅在 Layout 事件中刷新数据，确保 Layout 和 Repaint 使用相同的数据
            if (Event.current.type == EventType.Layout)
            {
                RefreshDisplayStats();
            }

            DrawSettingsSection();
            DrawToolbar();
            DrawStatsTable();
        }

        private void RefreshDisplayStats()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - _lastRefreshTime >= RefreshInterval)
            {
                _lastRefreshTime = currentTime;
                RefreshSortedStats();
            }

            // 复制到显示列表
            _displayStats.Clear();
            _displayStats.AddRange(_sortedStats);
        }

        private void DrawSettingsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            bool autoEnable = EditorGUILayout.Toggle(
                new GUIContent("Auto Enable On Play", "进入 Play 模式后自动开启 Command Profiling"),
                Settings.AutoEnableOnPlay
            );
            if (EditorGUI.EndChangeCheck())
            {
                Settings.AutoEnableOnPlay = autoEnable;
                Settings.Save();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(4);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // 启用/禁用 Profiling
            EditorGUI.BeginChangeCheck();
            bool enabled = GUILayout.Toggle(
                CommandSendHook.Enabled,
                "Enable Command Profiling",
                EditorStyles.toolbarButton,
                GUILayout.Width(160)
            );
            if (EditorGUI.EndChangeCheck())
            {
                CommandSendHook.Enabled = enabled;
            }

            GUILayout.FlexibleSpace();

            // Reset 按钮
            if (GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                CommandStatsStore.Reset();
                _sortedStats.Clear();
                _displayStats.Clear();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawStatsTable()
        {
            // 表头
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (DrawSortableHeader("Command Type", SortColumn.TypeName, 200))
            {
                SetSortColumn(SortColumn.TypeName);
            }

            if (DrawSortableHeader("Total Count", SortColumn.TotalCount, 100))
            {
                SetSortColumn(SortColumn.TotalCount);
            }

            if (DrawSortableHeader("Peak/Second", SortColumn.PeakPerSecond, 100))
            {
                SetSortColumn(SortColumn.PeakPerSecond);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // 表格内容
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (_displayStats.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    CommandSendHook.Enabled
                        ? "No commands recorded yet.  Send some commands in Play mode."
                        : "Enable Command Profiling to start recording.",
                    MessageType.Info
                );
            }
            else
            {
                for (int i = 0; i < _displayStats.Count; i++)
                {
                    var kvp = _displayStats[i];
                    DrawStatRow(kvp.Key, kvp.Value);
                }
            }

            EditorGUILayout.EndScrollView();

            // 状态栏
            DrawStatusBar();
        }

        private bool DrawSortableHeader(string label, SortColumn column, float width)
        {
            string displayLabel = label;
            if (_sortColumn == column)
            {
                displayLabel += _sortDescending ? " ▼" : " ▲";
            }

            return GUILayout.Button(displayLabel, EditorStyles.toolbarButton, GUILayout.Width(width));
        }

        private void SetSortColumn(SortColumn column)
        {
            if (_sortColumn == column)
            {
                _sortDescending = !_sortDescending;
            }
            else
            {
                _sortColumn = column;
                _sortDescending = true;
            }

            // 立即刷新排序
            RefreshSortedStats();
        }

        private void RefreshSortedStats()
        {
            _sortedStats.Clear();

            var stats = CommandStatsStore.Stats;
            if (stats == null || stats.Count == 0)
            {
                return;
            }

            _sortedStats.AddRange(stats);

            // 排序
            switch (_sortColumn)
            {
                case SortColumn.TypeName:
                    if (_sortDescending)
                        _sortedStats.Sort((a, b) => string.Compare(b.Key.Name, a.Key.Name, StringComparison.Ordinal));
                    else
                        _sortedStats.Sort((a, b) => string.Compare(a.Key.Name, b.Key.Name, StringComparison.Ordinal));
                    break;

                case SortColumn.TotalCount:
                    if (_sortDescending)
                        _sortedStats.Sort((a, b) => b.Value.TotalCount.CompareTo(a.Value.TotalCount));
                    else
                        _sortedStats.Sort((a, b) => a.Value.TotalCount.CompareTo(b.Value.TotalCount));
                    break;

                case SortColumn.PeakPerSecond:
                    if (_sortDescending)
                        _sortedStats.Sort((a, b) => b.Value.PeakPerSecond.CompareTo(a.Value.PeakPerSecond));
                    else
                        _sortedStats.Sort((a, b) => a.Value.PeakPerSecond.CompareTo(b.Value.PeakPerSecond));
                    break;
            }
        }

        private void DrawStatRow(Type commandType, CommandStat stat)
        {
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            // 类型名（可点击复制完整类型名）
            var typeNameContent = new GUIContent(commandType.Name, commandType.FullName);
            if (GUILayout.Button(typeNameContent, EditorStyles.label, GUILayout.Width(200)))
            {
                EditorGUIUtility.systemCopyBuffer = commandType.FullName;
                Debug.Log($"Copied to clipboard: {commandType.FullName}");
            }

            // TotalCount
            GUILayout.Label(stat.TotalCount.ToString(), GUILayout.Width(100));

            // PeakPerSecond
            GUILayout.Label(stat.PeakPerSecond.ToString(), GUILayout.Width(100));

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawStatusBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            string status = CommandSendHook.Enabled ? "● Recording" : "○ Stopped";
            Color originalColor = GUI.color;
            GUI.color = CommandSendHook.Enabled ? Color.green : Color.gray;
            GUILayout.Label(status, GUILayout.Width(80));
            GUI.color = originalColor;

            GUILayout.FlexibleSpace();

            GUILayout.Label($"Commands: {_displayStats.Count}", EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();
        }
    }
}