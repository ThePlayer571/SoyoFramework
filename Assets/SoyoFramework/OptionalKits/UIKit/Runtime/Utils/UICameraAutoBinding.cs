using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Utils
{
    [RequireComponent(typeof(Camera))]
    public class UICameraAutoBinding : MonoBehaviour
    {
        private void Awake()
        {
            var mainCam = GetComponent<Camera>();
            UIKit.PutUICameraIntoStack(mainCam);
        }
    }
}