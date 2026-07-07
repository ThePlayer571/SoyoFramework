using SoyoFramework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.UnityListener
{
    public class LateUpdateListener : MonoBehaviour
    {
        public EasyEvent onLateUpdate { get; } = new();
        private void LateUpdate()
        {
            onLateUpdate.Trigger();
        }
    }
}