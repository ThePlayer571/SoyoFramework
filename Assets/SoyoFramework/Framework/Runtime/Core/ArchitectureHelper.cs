using SoyoFramework.Framework.Runtime.LogKit;

namespace SoyoFramework.Framework.Runtime.Core
{
    public static class ArchitectureHelper
    {
        private static IArchitecture _defaultArchitecture;

        public static IArchitecture DefaultArchitecture
        {
            get => _defaultArchitecture;
            internal set
            {
                if (_defaultArchitecture != null)
                {
                    "设置失败：DefaultArchitecture已经被设置，不能重复设置".LogError();
                    return;
                }

                if (!value.Inited)
                {
                    "设置失败：DefaultArchitecture必须是已经初始化的Architecture".LogError();
                    return;
                }

                _defaultArchitecture = value;
            }
        }
    }
}