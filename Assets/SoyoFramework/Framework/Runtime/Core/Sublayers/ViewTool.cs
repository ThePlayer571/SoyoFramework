using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Sublayers
{
    /// <summary>
    /// <para>
    /// Sublayer：ViewTool
    /// </para>
    /// <para>
    /// 用于解决 ViewController 层逻辑复用的问题。
    /// </para>
    /// <para>约定：
    /// <list type="number">
    /// <item>建议 ViewTool 中只包含视图层面的封装，如果有逻辑封装，建议使用 Command </item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IViewTool :
        ISublayer, IViewControllerCanGet,
        ICanGetModel, ICanSendCommand
    {
    }

    public abstract class AbstractViewTool : AbstractSublayer, IViewTool
    {
    }

    public static class CanGetViewToolExtension
    {
        public static T GetViewTool<T>(this IViewControllerRule self)
            where T : class, IViewTool
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }

        public static T GetViewTool<T>(this IViewTool self)
            where T : class, IViewTool
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }
    }
}