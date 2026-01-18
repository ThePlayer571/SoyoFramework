namespace SoyoFramework.OptionalKits.UIKit.Runtime.Page
{
    public interface IUIPageLogic
    {
        bool TryHandleCommand(UICommand command);
        void OnInit();
        void OnClose();
    }

    public abstract class UIPageLogic : IUIPageLogic
    {
        protected UIPageLogic( IUIViewHost host)
        {
            Host = host;
        }
        
        protected IUIViewHost Host { get; }
        
        public abstract bool TryHandleCommand(UICommand command);
        public abstract void OnInit();
        public abstract void OnClose();
    }
}