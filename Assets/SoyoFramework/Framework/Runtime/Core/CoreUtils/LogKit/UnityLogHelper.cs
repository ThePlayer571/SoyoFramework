using SoyoFramework.Framework.Runtime.Core.CoreUtils.LogKit;

namespace SoyoFramework.Framework.Runtime.Utils.LogKit
{
    public static class UnityLogHelper
    {
        private static ILog _logger = new SoyoLogger();

        public static LogStrategy LogStrategy
        {
            get => _logger.LogStrategy;
            set => _logger.LogStrategy = value;
        }

        public static void LogInfo(this object self)
        {
            _logger.LogInfo(self);
        }

        public static void LogWarning(this object self)
        {
            _logger.LogWarning(self);
        }

        public static void LogError(this object self)
        {
            _logger.LogError(self);
        }

        public static void LogInfo(this object self, ILog logger)
        {
            logger.LogInfo(self);
        }

        public static void LogWarning(this object self, ILog logger)
        {
            logger.LogWarning(self);
        }

        public static void LogError(this object self, ILog logger)
        {
            logger.LogError(self);
        }
    }
}