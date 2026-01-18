using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
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