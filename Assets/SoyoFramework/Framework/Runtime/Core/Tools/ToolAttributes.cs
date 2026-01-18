using System;
using SoyoFramework.Framework.Runtime.Utils;

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
}