using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.FluentAPI
{
    public static class UnityTransformExtension
    {
        public static Transform LocalPosition(this Transform self, Vector3 localPos)
        {
            self.localPosition = localPos;
            return self;
        }

        public static Transform LocalPosition(this Transform self, float x, float y, float z)
        {
            self.localPosition = new Vector3(x, y, z);
            return self;
        }

        public static Transform LocalPosition(this Transform self, float x, float y)
        {
            var localPosition = self.localPosition;
            localPosition.x = x;
            localPosition.y = y;
            self.localPosition = localPosition;
            return self;
        }

        public static Transform LocalPositionX(this Transform self, float x)
        {
            var localPosition = self.localPosition;
            localPosition.x = x;
            self.localPosition = localPosition;
            return self;
        }

        public static Transform LocalPositionY(this Transform self, float y)
        {
            var localPosition = self.localPosition;
            localPosition.y = y;
            self.localPosition = localPosition;
            return self;
        }

        public static Transform LocalPositionZ(this Transform self, float z)
        {
            var localPosition = self.localPosition;
            localPosition.z = z;
            self.localPosition = localPosition;
            return self;
        }

        public static Transform LocalRotation(this Transform self, Quaternion localRotation)
        {
            self.localRotation = localRotation;
            return self;
        }

        public static Transform LocalEulerAnglesZ(this Transform self, float z)
        {
            var localEulerAngles = self.localEulerAngles;
            localEulerAngles.z = z;
            self.localEulerAngles = localEulerAngles;
            return self;
        }

        public static Transform LocalScale(this Transform self, Vector3 scale)
        {
            self.localScale = scale;
            return self;
        }

        public static Transform LocalScale(this Transform self, float xyz)
        {
            self.localScale = Vector3.one * xyz;
            return self;
        }

        public static Transform LocalScale(this Transform self, float x, float y, float z)
        {
            var localScale = self.localScale;
            localScale.x = x;
            localScale.y = y;
            localScale.z = z;
            self.localScale = localScale;
            return self;
        }

        public static Transform LocalScale(this Transform self, float x, float y)
        {
            var localScale = self.localScale;
            localScale.x = x;
            localScale.y = y;
            self.localScale = localScale;
            return self;
        }

        public static Transform LocalScaleX(this Transform self, float x)
        {
            var localScale = self.localScale;
            localScale.x = x;
            self.localScale = localScale;
            return self;
        }

        public static Transform LocalScaleY(this Transform self, float y)
        {
            var localScale = self.localScale;
            localScale.y = y;
            self.localScale = localScale;
            return self;
        }

        public static Transform LocalScaleZ(this Transform self, float z)
        {
            var localScale = self.localScale;
            localScale.z = z;
            self.localScale = localScale;
            return self;
        }

        public static Transform RemoveAllChildren(this Transform self)
        {
            for (int i = self.childCount - 1; i >= 0; i--)
            {
                var child = self.GetChild(i);
                Object.Destroy(child.gameObject);
            }

            return self;
        }
    }
}