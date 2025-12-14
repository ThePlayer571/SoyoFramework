using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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