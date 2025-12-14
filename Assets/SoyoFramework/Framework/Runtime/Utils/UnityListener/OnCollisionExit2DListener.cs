using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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

