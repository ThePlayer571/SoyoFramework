using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    /// <summary>
    /// <para>
    /// Tool 层：Service
    /// </para>
    /// <para>
    /// 主要用于解决 System 层逻辑复用的问题，例如需要对数据进行复杂修改操作时，可通过 Service 统一处理。
    /// 也适合封装特定操作的唯一入口。例如，若多个 Command 和 System 均需执行种植植物的操作，此时无法用 Command 封装
    /// </para>
    /// <para>
    /// 约定：
    /// <list type="number">
    /// <item>建议同一个 Service 中各操作的抽象程度应保持一致；</item>
    /// <item>不建议 Service 存在任何状态数据。 </item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IService :
        ITool, ISystemCanGet,
        ICanGetModel, ICanSendEvent
    {
    }

    public abstract class AbstractService : AbstractTool, IService
    {
    }

    public static class CanGetServiceExtension
    {
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