namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces
{
    public interface IViewHelperBase
    {
        /// <summary>
        /// 重置View至当前状态（不考虑条件）
        /// </summary>
        void ForceResetView();

        /// <summary>
        /// 更新View
        /// </summary>
        void Update();
    }

    public interface IViewHelperBase<in TData> : IViewHelperBase
    {
        /// <summary>
        /// 更新View
        /// </summary>
        /// <param name="data"></param>
        void Update(TData data);

        /// <summary>
        /// 重置View至当前状态（不考虑条件）
        /// </summary>
        void ForceResetView(TData data);
    }
}