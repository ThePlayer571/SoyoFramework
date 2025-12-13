using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    public interface IService :
        ITool,
        ICanGetModel, ICanSendEvent
    {
    }

    public abstract class AbstractService : AbstractTool, IService
    {
    }

    public static class CanGetServiceExtension
    {
        public static T GetService<T>(this ISystem self)
            where T : class, IService
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetService<T>(this IService self)
            where T : class, IService
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}