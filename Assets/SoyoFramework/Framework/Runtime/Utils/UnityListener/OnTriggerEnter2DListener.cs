using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.Others
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

