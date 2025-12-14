using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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