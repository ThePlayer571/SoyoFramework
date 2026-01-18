using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.UnityListener
{
    public class OnDestroyListener : MonoBehaviour
    {
        public EasyEvent onDestroy { get; } = new();
        
        private void OnDestroy()
        {
            onDestroy.Trigger();
        }
    }
}