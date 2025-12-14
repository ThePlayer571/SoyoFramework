using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
{
    public class OnTriggerEnter2DListener : MonoBehaviour
    {
        public EasyEvent<Collider2D> onTriggerEnter2D { get; } = new();
        private void OnTriggerEnter2D(Collider2D collider)
        {
            onTriggerEnter2D.Trigger(collider);
        }
    }
}

