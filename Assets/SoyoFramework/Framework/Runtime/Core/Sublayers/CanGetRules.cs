namespace SoyoFramework.Framework.Runtime.Core.Sublayers
{
    public interface IModelCanGet : ISublayer
    {
    }

    public interface ISystemCanGet : ISublayer
    {
    }

    public interface IViewControllerCanGet : ISublayer
    {
    }

    public static class CanGetRulesExtensions
    {
        public static T GetSublayer<T>(this IViewControllerRule self) where T : class, IViewControllerCanGet
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }


        public static T GetSublayer<T>(this ICommandRule self) where T : class, ISystemCanGet
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }

        public static T GetVCTool<T>(this ICommandRule self) where T : class, IViewControllerCanGet
        {
            return self.RelyingArchitecture.GetSublayer<T>();
        }
    }
}