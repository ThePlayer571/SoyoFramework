using UnityEngine;

namespace SoyoFramework.ToolKits.FluentAPI
{
    public static class UnityObjectExtension
    {
        public static T Instantiate<T>(this T selfObj) where T : Object
        {
            return Object.Instantiate(selfObj);
        }

        public static T Instantiate<T>(this T selfObj, Transform parent)
            where T : Object
        {
            return Object.Instantiate(selfObj, parent);
        }

        public static T Instantiate<T>(this T selfObj, Transform parent, bool worldPositionStays)
            where T : Object
        {
            return Object.Instantiate(selfObj, parent, worldPositionStays);
        }

        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation)
            where T : Object
        {
            return Object.Instantiate(selfObj, position, rotation);
        }

        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : Object
        {
            return Object.Instantiate(selfObj, position, rotation, parent);
        }


        public static void DestroySelf<T>(this T selfObj) where T : Object
        {
            Object.Destroy(selfObj);
        }

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : Object
        {
            Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }
    }
}