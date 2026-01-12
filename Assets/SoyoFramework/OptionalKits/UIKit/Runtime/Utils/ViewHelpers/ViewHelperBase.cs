using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers
{
    public abstract class ViewHelperBase : IViewHelperBase
    {
        #region 接口实现

        public abstract void ForceResetView();
        public abstract void Update();

        #endregion

        #region Log支持

        protected ILog Logger { get; } = new ViewHelperLogger();

        private class ViewHelperLogger : ILog
        {
            public LogStrategy LogStrategy { get; set; } = LogStrategy.All;

            public void LogInfo(object message)
            {
                if (LogStrategy == LogStrategy.All)
                    UnityEngine.Debug.Log("[ViewHelper] " + message);
            }

            public void LogWarning(object message)
            {
                if (LogStrategy is LogStrategy.All or LogStrategy.WarningAndError)
                    UnityEngine.Debug.LogWarning("[ViewHelper] " + message);
            }

            public void LogError(object message)
            {
                UnityEngine.Debug.LogError("[ViewHelper] " + message);
            }
        }

        #endregion
    }

    public abstract class ViewHelperBase<TData> : ViewHelperBase, IViewHelperBase<TData>
    {
        public abstract void Update(TData data);
        public abstract void ForceResetView(TData data);
        public override void Update() => Update(default);
        public override void ForceResetView() => ForceResetView(default);
    }
}