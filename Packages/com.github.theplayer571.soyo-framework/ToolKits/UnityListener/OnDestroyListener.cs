using SoyoFramework.Utils;
using UnityEngine;

namespace SoyoFramework.ToolKits.UnityListener
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