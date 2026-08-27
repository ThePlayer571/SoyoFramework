using UnityEngine;

namespace SoyoFramework
{
    public static class GlobalArchitecture
    {
        private static IArchitecture? _instance;

        public static IArchitecture? Instance
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
                        Debug.LogError($"在已有{nameof(GlobalArchitecture)}实例的情况下尝试设置新的实例，被阻断");
                    }
                }
            }
        }
    }
}