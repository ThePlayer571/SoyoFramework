using SoyoFramework.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.UnityListener
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