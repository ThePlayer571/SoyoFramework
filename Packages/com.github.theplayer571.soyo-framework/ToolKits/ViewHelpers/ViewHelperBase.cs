using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.ToolKits.ViewHelpers
{
    public abstract class ViewHelperBase : IViewHelperBase
    {
        #region 接口实现

        public abstract void ForceResetView();
        public abstract void Update();

        #endregion

        #region Log支持

        protected ILog Logger { get; } = new PrefixLogger("ViewHelper", LogStrategy.All);

        #endregion
    }

    public abstract class ViewHelperBase<TData> : ViewHelperBase, IViewHelperBase<TData>
    {
        public abstract void Update(TData data);
        public abstract void ForceResetView(TData data);
        public override void Update() => Update(default!);
        public override void ForceResetView() => ForceResetView(default!);
    }
}