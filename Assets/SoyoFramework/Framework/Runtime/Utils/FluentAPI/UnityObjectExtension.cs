using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils.FluentAPI
{
    public static class UnityObjectExtension
    {
        public static T Instantiate<T>(this T selfObj) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj);
        }

        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation);
        }
        public static T Instantiate<T>(this T selfObj, Vector2 position, Quaternion rotation)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation);
        }

        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation, parent);
        }

        public static T Name<T>(this T selfObj, string name) where T : UnityEngine.Object
        {
            selfObj.name = name;
            return selfObj;
        }

        public static void DestroySelf<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj);
        }

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }
    }
}