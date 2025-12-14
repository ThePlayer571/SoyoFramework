using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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