using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;

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
                    Debug.LogError("设置失败：DefaultArchitecture已经被设置，不能重复设置");
                    return;
                }

                if (!value.Inited)
                {
                    Debug.LogError("设置失败：DefaultArchitecture必须是已经初始化的Architecture");
                    return;
                }

                _defaultArchitecture = value;
            }
        }
    }
}