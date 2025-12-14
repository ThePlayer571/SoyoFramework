using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.Others
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