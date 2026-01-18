using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
{
    public class OnCollisionExit2DListener : MonoBehaviour
    {
        public EasyEvent<Collision2D> onCollisionExit2D { get; } = new();
        private void OnCollisionExit2D(Collision2D collision)
        {
            onCollisionExit2D.Trigger(collision);
        }
    }
}

