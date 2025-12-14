using Debug = UnityEngine.Debug;

namespace SoyoFramework.Framework.Runtime.Utils.LogKit
{
    public static class UnityLogExtension
    {
        public static void LogInfo(this object self)
        {
            Debug.Log(self);
        }

        public static void LogWarning(this object self)
        {
            Debug.LogWarning(self);
        }

        public static void LogError(this object self)
        {
            Debug.LogError(self);
        }
    }
}