using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.Others
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