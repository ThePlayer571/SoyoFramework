using System;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.FluentAPI
{
    public static class MathExtensions
    {
        public static float Sign(this in float self)
        {
            return Mathf.Sign(self);
        }

        public static int Sign(this in int self)
        {
            return Math.Sign(self);
        }

        public static bool Approximately(this in float self, in float other)
        {
            return Mathf.Approximately(self, other);
        }

        public static bool Approximately(this in Vector2 self, in Vector2 other)
        {
            return Mathf.Approximately(self.x, other.x) && Mathf.Approximately(self.y, other.y);
        }

        public static bool Approximately(this in Vector3 self, in Vector3 other)
        {
            return Mathf.Approximately(self.x, other.x) && Mathf.Approximately(self.y, other.y) &&
                   Mathf.Approximately(self.z, other.z);
        }
    }
}