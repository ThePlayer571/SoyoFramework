namespace SoyoFramework.OptionalKits.UIKit.Runtime.Page
{
    public abstract class UIModule
    {
        #region Protected 子类可用

        protected IUIViewHost Host { get; private set; }

        protected UIModule(IUIViewHost host)
        {
            Host = host;
        }

        #endregion
    }
}