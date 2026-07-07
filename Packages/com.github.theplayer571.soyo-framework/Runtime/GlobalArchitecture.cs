using UnityEngine;

namespace SoyoFramework
{
    public static class GlobalArchitecture
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
                        Debug.LogError("在已有GlobalArchitecture的情况下，尝试设置GlobalArchitecture，被阻断");
                    }
                }
            }
        }
    }
}