using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
{
    public class OnTriggerStay2DListener : MonoBehaviour
    {
        public EasyEvent<Collider2D> onTriggerStay2D { get; } = new();
        private void OnTriggerStay2D(Collider2D collider)
        {
            onTriggerStay2D.Trigger(collider);
        }
    }
}

