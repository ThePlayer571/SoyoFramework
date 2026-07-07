using UnityEngine;

namespace SoyoFramework.ToolKits.FluentAPI
{
    public static class LayerMaskUtility
    {
        public static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            var objLayerMask = 1 << layer;
            return (layerMask.value & objLayerMask) == objLayerMask;
        }
    }
}