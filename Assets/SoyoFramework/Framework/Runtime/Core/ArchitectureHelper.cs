using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core
{
    public static class ArchitectureHelper
    {
        public static void InitAsDefault(this IArchitecture architecture)
        {
            architecture.Init();
            DefaultArchitecture = architecture;
        }

        private static IArchitecture _defaultArchitecture;

        public static IArchitecture DefaultArchitecture
        {
            get => _defaultArchitecture;
            private set
            {
                if (_defaultArchitecture == null || !_defaultArchitecture.Inited)
                {
                    _defaultArchitecture = value;
                }
                else
                {
                    Debug.LogError("在已有默认Architecture的情况下，尝试设置DefaultArchitecture，被阻断");
                }
            }
        }
    }
}