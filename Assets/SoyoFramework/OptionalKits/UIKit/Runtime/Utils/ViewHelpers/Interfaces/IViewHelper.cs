namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils.ViewHelpers.Interfaces
{
    public interface IViewHelper : IViewHelperBase
    {
        void Show();
        void Hide();
        bool IsShow { get; }
    }

    public interface IViewHelper<in TData> : IViewHelperBase<TData>, IViewHelper
    {
        void Show(TData data);
        void Hide(TData data);
    }

    public static class IViewHelperExtensions
    {
        public static void SetView(this IViewHelper viewHelper, bool isShow)
        {
            if (isShow) viewHelper.Show();
            else viewHelper.Hide();
        }
        
        public static void ToggleView(this IViewHelper viewHelper)
        {
            if (viewHelper.IsShow) viewHelper.Hide();
            else viewHelper.Show();
        }

        public static void SetView<TData>(this IViewHelper<TData> viewHelper, bool isShow, TData data)
        {
            if (isShow) viewHelper.Show(data);
            else viewHelper.Hide(data);
        }

        public static void ToggleView<TData>(this IViewHelper<TData> viewHelper, TData data)
        {
            if (viewHelper.IsShow) viewHelper.Hide(data);
            else viewHelper.Show(data);
        }
    }
}