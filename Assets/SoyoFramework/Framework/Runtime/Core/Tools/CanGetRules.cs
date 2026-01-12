namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    public interface IModelTool : ITool
    {
    }

    public interface ISystemTool : ITool
    {
    }
    
    public interface IViewControllerTool : ITool
    {
    }

    public static class CanGetRulesExtensions
    {
        public static T GetTool<T>(this ISystemRule self) where T : class, IModelTool
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetTool<T>(this IModelRule self) where T : class, ISystemTool
        {
            return self.RelyingArchitecture.GetTool<T>();
        }

        public static T GetTool<T>(this IViewControllerRule self) where T : class, IViewControllerTool
        {
            return self.RelyingArchitecture.GetTool<T>();
        }
    }
}