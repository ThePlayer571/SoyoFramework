using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
{
    public class OnCollisionEnter2DListener : MonoBehaviour
    {
        public EasyEvent<Collision2D> onCollisionEnter2D { get; } = new();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter2D.Trigger(collision);
        }
    }
}