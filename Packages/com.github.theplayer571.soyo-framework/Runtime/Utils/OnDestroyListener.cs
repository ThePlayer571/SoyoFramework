using UnityEngine;

namespace SoyoFramework.Utils
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