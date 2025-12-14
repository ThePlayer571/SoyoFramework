using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.Others
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