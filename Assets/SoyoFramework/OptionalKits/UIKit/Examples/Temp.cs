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
        private void Awake()
        {
            "call Awake".LogInfo();
            c = new ClassA(5);
            list.Add(c);
            list.Add(c);
        }

        [SerializeReference] public List<ClassA> list;

        [NonSerialized] private ClassA c;

        public bool t;

        private void Update()
        {
            if (t)
            {
                t = false;
                c.x++;
                $"c.x={c.x}".LogInfo();
            }
        }
    }

    [Serializable]
    public class ClassA
    {
        public int x;
        public int y;

        public ClassA()
        {
            "call ClassA Constructor no para".LogInfo();
            x = 1;
            y = 1;
        }

        public ClassA(int a)
        {
            "call ClassA Constructor with para".LogInfo();
            x = 2;
            y = 3;
        }
    }


    // [Serializable]
    // public class A : ISerializationCallbackReceiver
    // {
    //     public int x;
    //     public int y;
    //     [NonSerialized] public B SomeB;
    //
    //     public A(int a)
    //     {
    //         SomeB = new B();
    //         x = 0;
    //         y = 0;
    //         "call A Constructor".LogInfo();
    //     }
    //
    //     public A()
    //     {
    //         SomeB = new B();
    //         "call A Constructor no para".LogInfo();
    //     }
    //
    //     public void OnBeforeSerialize()
    //     {
    //         $"call OnBeforeSerialize, null?:{SomeB == null}".LogInfo();
    //     }
    //
    //     public void OnAfterDeserialize()
    //     {
    //         $"call OnAfterDeserialize, null?:{SomeB == null}".LogInfo();
    //     }
    // }
    //
    // public class B
    // {
    // }
}