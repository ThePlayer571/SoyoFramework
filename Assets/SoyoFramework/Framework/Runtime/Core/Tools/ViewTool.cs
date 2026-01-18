using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    /// <summary>
    /// <para>
    /// Tool 层：ViewTool
    /// </para>
    /// <para>
    /// 用于解决 ViewController 层逻辑复用的问题。
    /// </para>
    /// </summary>
    public interface IViewTool :
        ITool, IViewControllerCanGet,
        ICanGetModel, ICanSendCommand
    {
    }

    public abstract class ViewTool : AbstractTool, IViewTool
    {
    }
    
    public static class CanGetViewToolExtension
    {
        public static T GetViewTool<T>(this IViewControllerRule self)
            where T : class, IViewTool
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetViewTool<T>(this IViewTool self)
            where T : class, IViewTool
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}