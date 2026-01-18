using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
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

