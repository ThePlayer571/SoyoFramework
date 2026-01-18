namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    public interface IModelCanGet : ITool
    {
    }

    public interface ISystemCanGet : ITool
    {
    }

    public interface IViewControllerCanGet : ITool
    {
    }

    public static class CanGetRulesExtensions
    {
        public static T GetTool<T>(this ISystemRule self) where T : class, IModelCanGet
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetTool<T>(this IModelRule self) where T : class, ISystemCanGet
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetTool<T>(this IViewControllerRule self) where T : class, IViewControllerCanGet
        {
            return self.RelyingArchitecture.GetTool<T>();
        }


        public static T GetTool<T>(this ICommandRule self) where T : class, ISystemCanGet
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetVCTool<T>(this ICommandRule self) where T : class, IViewControllerCanGet
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}