using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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