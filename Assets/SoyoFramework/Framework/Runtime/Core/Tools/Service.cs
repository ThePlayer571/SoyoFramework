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

    public interface ICanGetService : ICanRelyOnArchitecture
    {
    }

    public static class CanGetServiceExtension
    {
        public static T GetService<T>(this ICanGetService self)
            where T : class, IService
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetService<T>(this ISystemRule self)
            where T : class, IService
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetService<T>(this IService self)
            where T : class, IService
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetService<T>(this ICommandRule self)
            where T : class, IService
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

    }
}