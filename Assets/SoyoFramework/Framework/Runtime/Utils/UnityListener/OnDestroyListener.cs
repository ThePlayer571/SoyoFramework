using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.UnityListener
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