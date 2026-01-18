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
                if (_defaultArchitecture == null)
                {
                    if (!value.Inited)
                    {
                        Debug.LogError("尝试将未初始化的Architecture设置为DefaultArchitecture，被阻断");
                        return;
                    }

                    _defaultArchitecture = value;
                }
                else
                {
                    if (value == null)
                    {
                        _defaultArchitecture = null;
                    }
                    else
                    {
                        Debug.LogError("在已有默认Architecture的情况下，尝试设置DefaultArchitecture，被阻断");
                    }

                    return;
                }

                if (_defaultArchitecture != null)
                {
                }
            }
        }
    }
}