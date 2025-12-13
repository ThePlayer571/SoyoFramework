using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    public interface IUtility :
        ITool
    {
    }

    public abstract class AbstractUtility : AbstractTool
    {
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this IModel self)
            where T : class, IUtility
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}