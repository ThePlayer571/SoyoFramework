using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
{
    public class UpdateListener : MonoBehaviour
    {
        public EasyEvent onUpdate { get; } = new();
        private void Update()
        {
            onUpdate.Trigger();
        }
    }
}