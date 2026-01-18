using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    /// <summary>
    /// <para>Tool层：ViewModel</para>
    /// <para>
    /// 模仿 MVVM 架构中的 ViewModel 概念，用于封装 ViewController 层与 Model 层之间的数据交互逻辑。
    /// </para>
    /// <para>
    /// 为什么出现这个 Tool？：框架本体规定，VC层能获取 Model，却不建议直接操作 Model，这是为了使用方便而妥协。
    /// 如果你想要更好的封装，可以使用ViewModel。
    /// </para>
    /// </summary>
    public interface IViewModel :
        ITool, IViewControllerCanGet,
        ICanGetModel
    {
    }
    
    
    public class AbstractViewModel : AbstractTool, IViewModel
    {
        
    }
    
    public static class CanGetViewModelExtension
    {
        public static T GetViewModel<T>(this IViewControllerRule self)
            where T : class, IViewModel
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetViewModel<T>(this IViewModel self)
            where T : class, IViewModel
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}