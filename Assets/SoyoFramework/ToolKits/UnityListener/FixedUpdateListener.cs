using SoyoFramework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.UnityListener
{
    public class FixedUpdateListener : MonoBehaviour
    {
        public EasyEvent onFixedUpdate { get; } = new();

        private void FixedUpdate()
        {
            onFixedUpdate.Trigger();
        }
    }
}