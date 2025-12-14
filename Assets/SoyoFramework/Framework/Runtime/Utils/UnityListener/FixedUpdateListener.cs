using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.Others
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