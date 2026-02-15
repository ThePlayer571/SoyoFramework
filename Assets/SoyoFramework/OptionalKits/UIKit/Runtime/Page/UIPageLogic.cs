namespace SoyoFramework.OptionalKits.UIKit.Runtime.Page
{
    public interface IUIPageLogic
    {
        bool HandleUICommand(UICommand command);
        bool HandleUICommand<TResult>(UICommand<TResult> command, out TResult result);
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
        
        public abstract bool HandleUICommand(UICommand command);
        public abstract bool HandleUICommand<TResult>(UICommand<TResult> command, out TResult result);
        public abstract void OnInit();
        public abstract void OnClose();
    }
}