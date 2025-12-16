using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.CommandProfiler
{
    internal static class CommandStatsStore
    {
        // 对外
        internal static IReadOnlyDictionary<Type, CommandStat> Stats => _stats;

        internal static void Reset()
        {
            _stats.Clear();
            _currentSecond = 0;
        }

        internal static void Record(Type commandType)
        {
            int nowSecond = (int)Time.realtimeSinceStartup;

            if (nowSecond != _currentSecond)
            {
                // 刷新上一秒的统计数据
                foreach (var s in _stats.Values)
                {
                    if (s.CurrentSecondCount > s.PeakPerSecond)
                        s.PeakPerSecond = s.CurrentSecondCount;
                    s.CurrentSecondCount = 0;
                }

                _currentSecond = nowSecond;
            }

            // 获取或创建 CommandStat
            if (!_stats.TryGetValue(commandType, out var stat))
            {
                stat = new CommandStat();
                _stats.Add(commandType, stat);
            }

            stat.TotalCount++;
            stat.CurrentSecondCount++;
        }

        // 内部
        private static readonly Dictionary<Type, CommandStat> _stats = new();

        private static int _currentSecond;
    }
}