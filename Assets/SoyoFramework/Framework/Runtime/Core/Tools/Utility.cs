using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    public interface IUtility : IModelTool
    {
    }

    public abstract class AbstractUtility : AbstractTool, IUtility
    {
    }

    public interface ICanGetUtility : ICanRelyOnArchitecture
    {
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self)
            where T : class, IUtility
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetUtility<T>(this IModel self)
            where T : class, IUtility
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}