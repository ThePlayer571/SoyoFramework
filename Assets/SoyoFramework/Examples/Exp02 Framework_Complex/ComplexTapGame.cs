using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Core.SuperLayers;
using UnityEngine;

namespace SoyoFramework.Examples.Exp02_Framework_Complex
{
    public class ComplexTapGame : AbstractArchitecture
    {
        protected override void OnInit()
        {
        }

        protected override void OnDeinit()
        {
        }
    }


    [SuperLayer("System + Model")]
    public class HandSystem : AbstractSystem, IModel
    {
        // 写逻辑
    }
}