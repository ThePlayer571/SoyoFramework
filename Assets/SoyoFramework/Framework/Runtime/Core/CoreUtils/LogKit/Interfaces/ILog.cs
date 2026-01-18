using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;

namespace SoyoFramework.Framework.Runtime.Utils.LogKit
{
    public interface ILog : ILogStrategy
    {
        void LogInfo(object message);
        void LogWarning(object message);
        void LogError(object message);
    }
}