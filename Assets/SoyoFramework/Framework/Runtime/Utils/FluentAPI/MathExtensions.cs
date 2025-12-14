using System;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.FluentAPI
{
    public static class MathExtensions
    {
        public static float Sign(this float self)
        {
            return Mathf.Sign(self);
        }

        public static int Sign(this int self)
        {
            return Math.Sign(self);
        }

        public static bool Approximately(this float self, float other)
        {
            return Mathf.Approximately(self, other);
        }
    }
}