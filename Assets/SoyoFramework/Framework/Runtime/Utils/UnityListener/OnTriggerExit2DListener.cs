using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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

