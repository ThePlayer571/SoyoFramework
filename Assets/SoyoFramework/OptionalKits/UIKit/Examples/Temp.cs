using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    [ExecuteAlways]
    public class Temp : MonoBehaviour
    {
        public BindableProperty<A> t;
    }

    [Serializable]
    public struct A
    {
        public int a;
        public int b;
    }
}