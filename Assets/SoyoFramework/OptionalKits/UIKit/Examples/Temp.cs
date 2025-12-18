using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class Temp : MonoBehaviour
    {
        [SerializeField] public EasyEvent<A> e;

        private void Awake()
        {
            // e.Register((x, y, z) => $"Event Triggered with value: {x};{y};{z}".LogInfo());
            e.Register(x => $"Event Triggered with value: {x}".LogInfo());
        }
    }

    [Serializable]
    public struct A
    {
        public int a;
        public int b;

        public override string ToString()
        {
            return $"{a},{b}";
        }
    }
}