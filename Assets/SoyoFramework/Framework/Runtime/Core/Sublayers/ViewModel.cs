using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Sublayers
{
    /// <summary>
    /// <para>Sublayer：ViewModel</para>
    /// <para>
    /// 模仿 MVVM 架构中的 ViewModel 概念，用于封装 ViewController 层与 Model 层之间的数据交互逻辑。
    /// </para>
    /// <para>
    /// 为什么出现这个 Sublayer？：框架本体规定，VC层能获取 Model，却不建议直接操作 Model，这是为了方便而妥协。
    /// 如果你想要更好的封装，可以使用ViewModel。
    /// </para>
    /// </summary>
    public interface IViewModel :
        ISublayer, IViewControllerCanGet
    {
    }
    
    
    public class AbstractViewModel : AbstractSublayer, IViewModel
    {
        
    }
    
    public static class CanGetViewModelExtension
    {
        public static T GetViewModel<T>(this IViewControllerRule self)
            where T : class, IViewModel
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }

        public static T GetViewModel<T>(this IViewModel self)
            where T : class, IViewModel
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }
    }
}