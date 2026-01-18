using System.Linq;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.FluentAPI
{
    public static class UnityGameObjectExtension
    {
        public static GameObject DestroySelf(this GameObject selfObj)
        {
            Object.Destroy(selfObj);
            return null;
        }

        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var comp = self.GetComponent<T>();
            return comp ? comp : self.AddComponent<T>();
        }
        
        public static T GetOrAddComponent<T>(this Component self) where T : Component
        {
            var comp = self.GetComponent<T>();
            return comp ? comp : self.gameObject.AddComponent<T>();
        }

        public static bool IsInLayerMask(this GameObject self, LayerMask layerMask)
        {
            return LayerMaskUtility.IsInLayerMask(self.layer, layerMask);
        }

        public static GameObject LocalPosition(this GameObject self, Vector3 localPos)
        {
            self.transform.localPosition = localPos;
            return self;
        }

        public static GameObject LocalPosition(this GameObject self, float x, float y, float z)
        {
            self.transform.localPosition = new Vector3(x, y, z);
            return self;
        }

        public static GameObject LocalPosition(this GameObject self, float x, float y)
        {
            var localPosition = self.transform.localPosition;
            localPosition.x = x;
            localPosition.y = y;
            self.transform.localPosition = localPosition;
            return self;
        }

        public static GameObject LocalPositionX(this GameObject self, float x)
        {
            var localPosition = self.transform.localPosition;
            localPosition.x = x;
            self.transform.localPosition = localPosition;
            return self;
        }

        public static GameObject LocalPositionY(this GameObject self, float y)
        {
            var localPosition = self.transform.localPosition;
            localPosition.y = y;
            self.transform.localPosition = localPosition;
            return self;
        }

        public static GameObject LocalPositionZ(this GameObject self, float z)
        {
            var localPosition = self.transform.localPosition;
            localPosition.z = z;
            self.transform.localPosition = localPosition;
            return self;
        }

        public static GameObject LocalRotation(this GameObject self, Quaternion localRotation)
        {
            self.transform.localRotation = localRotation;
            return self;
        }

        public static GameObject LocalScale(this GameObject self, Vector3 scale)
        {
            self.transform.localScale = scale;
            return self;
        }

        public static GameObject LocalScale(this GameObject self, float xyz)
        {
            self.transform.localScale = Vector3.one * xyz;
            return self;
        }

        public static GameObject LocalScale(this GameObject self, float x, float y, float z)
        {
            var localScale = self.transform.localScale;
            localScale.x = x;
            localScale.y = y;
            localScale.z = z;
            self.transform.localScale = localScale;
            return self;
        }

        public static GameObject LocalScale(this GameObject self, float x, float y)
        {
            var localScale = self.transform.localScale;
            localScale.x = x;
            localScale.y = y;
            self.transform.localScale = localScale;
            return self;
        }

        public static GameObject LocalScaleX(this GameObject self, float x)
        {
            var localScale = self.transform.localScale;
            localScale.x = x;
            self.transform.localScale = localScale;
            return self;
        }

        public static GameObject LocalScaleY(this GameObject self, float y)
        {
            var localScale = self.transform.localScale;
            localScale.y = y;
            self.transform.localScale = localScale;
            return self;
        }

        public static GameObject LocalScaleZ(this GameObject self, float z)
        {
            var localScale = self.transform.localScale;
            localScale.z = z;
            self.transform.localScale = localScale;
            return self;
        }

        public static void SetActive(this Component self, bool active)
        {
            self.gameObject.SetActive(active);
        }

        public static bool CompareTag(this Component self, params string[] tags)
        {
            return tags.Any(self.CompareTag);
        }

        public static bool CompareTag(this GameObject self, params string[] tags)
        {
            return tags.Any(self.CompareTag);
        }
    }
}