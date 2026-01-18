using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
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