using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.Others
{
    public class OnCollisionStay2DListener : MonoBehaviour
    {
        public EasyEvent<Collision2D> onCollisionStay2D { get; } = new();
        private void OnCollisionStay2D(Collision2D collision)
        {
            onCollisionStay2D.Trigger(collision);
        }
    }
}

