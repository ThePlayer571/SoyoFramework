using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.LogKit
{
    public class PrefixLogger : ILog
    {
        public LogStrategy LogStrategy { get; set; } = LogStrategy.All;
        private readonly string _prefix;

        public PrefixLogger(string prefix)
        {
            _prefix = prefix;
        }

        public PrefixLogger(string prefix, LogStrategy strategy)
        {
            _prefix = prefix;
            LogStrategy = strategy;
        }

        public void LogInfo(object message)
        {
            if (LogStrategy == LogStrategy.All)
                Debug.Log($"{_prefix} {message}");
        }

        public void LogWarning(object message)
        {
            if (LogStrategy is LogStrategy.All or LogStrategy.WarningAndError)
                Debug.LogWarning($"{_prefix} {message}");
        }

        public void LogError(object message)
        {
            Debug.LogError($"{_prefix} {message}");
        }
    }
}