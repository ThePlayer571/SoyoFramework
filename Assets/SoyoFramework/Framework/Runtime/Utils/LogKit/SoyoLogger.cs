using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.LogKit
{
    public interface ILogStrategy
    {
        LogStrategy LogStrategy { get; set; }
    }

    public interface ILog : ILogStrategy
    {
        void LogInfo(object message);
        void LogWarning(object message);
        void LogError(object message);
    }

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

    public enum LogStrategy
    {
        All, // 显示 Info + Warning + Error
        WarningAndError, // 显示 Warning + Error
        ErrorOnly // 只显示 Error
    }
}