using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.LogKit
{
    public class SoyoLogger : ILog
    {
        public LogStrategy LogStrategy { get; set; } = LogStrategy.All;

        public SoyoLogger()
        {
        }

        public SoyoLogger(LogStrategy strategy)
        {
            LogStrategy = strategy;
        }

        public void LogInfo(object message)
        {
            if (LogStrategy == LogStrategy.All)
                Debug.Log(message);
        }

        public void LogWarning(object message)
        {
            if (LogStrategy is LogStrategy.All or LogStrategy.WarningAndError)
                Debug.LogWarning(message);
        }

        public void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}