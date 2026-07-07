using SoyoFramework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.UnityListener
{
    public class OnTriggerExit2DListener : MonoBehaviour
    {
        public EasyEvent<Collider2D> onTriggerExit2D { get; } = new();
        private void OnTriggerExit2D(Collider2D collider)
        {
            onTriggerExit2D.Trigger(collider);
        }
    }
}

