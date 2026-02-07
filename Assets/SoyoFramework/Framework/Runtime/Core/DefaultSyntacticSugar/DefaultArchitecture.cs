using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar
{
    public static class DefaultArchitecture
    {
        private static IArchitecture _instance;

        public static IArchitecture Instance
        {
            get => _instance;
            internal set
            {
                if (value == null)
                {
                    _instance = null;
                }
                else
                {
                    if (_instance == null || !_instance.Inited)
                    {
                        _instance = value;
                    }
                    else
                    {
                        Debug.LogError("在已有默认Architecture的情况下，尝试设置DefaultArchitecture，被阻断");
                    }
                }
            }
        }
    }
}