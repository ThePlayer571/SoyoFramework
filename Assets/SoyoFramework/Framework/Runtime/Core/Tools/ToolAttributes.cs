using System;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;

namespace SoyoFramework.Framework.Runtime.Core.Tools
{
    /// <summary>
    /// 用于Tool，标记Tool依赖的层级
    /// </summary>
    public class RelyingLayerAttribute : Attribute
    {
        public SoyoLayer Layer { get; }

        public RelyingLayerAttribute(SoyoLayer layer)
        {
            Layer = layer;
        }
    }

    /// <summary>
    /// 用于Tool，标记依赖多个层级的Tool
    /// </summary>
    public class SuperToolAttribute : RelyingLayerAttribute
    {
        public SuperToolAttribute(SoyoLayer layer, params SoyoLayer[] layers) : base(layer)
        {
        }
    }
}