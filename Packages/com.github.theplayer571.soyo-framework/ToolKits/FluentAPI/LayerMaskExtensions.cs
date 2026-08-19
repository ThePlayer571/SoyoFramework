using System.Diagnostics.Contracts;
using UnityEngine;

namespace SoyoFramework.ToolKits.FluentAPI
{
    public static class LayerMaskUtility
    {
        [Pure]
        public static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            var objLayerMask = 1 << layer;
            return (layerMask.value & objLayerMask) == objLayerMask;
        }
    }

    public static class LayerMaskExtensions
    {
        [Pure]
        public static bool Contains(this LayerMask self, int layer)
        {
            return LayerMaskUtility.IsInLayerMask(layer, self);
        }

        [Pure]
        public static bool IsInLayerMask(this Collider2D self, LayerMask layerMask)
        {
            return LayerMaskUtility.IsInLayerMask(self.gameObject.layer, layerMask);
        }

        [Pure]
        public static bool IsInLayerMask(this Collider2D self, int layerMask)
        {
            return self.IsInLayerMask((LayerMask)layerMask);
        }
    }
}