using UnityEngine;

namespace SoyoFramework.UIKit.Runtime.Utils
{
    [RequireComponent(typeof(Camera))]
    public class UICameraAutoBinding : MonoBehaviour
    {
        private void Start()
        {
            if (UIKit.UIRoot.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                var mainCam = GetComponent<Camera>();
                UIKit.PutUICameraIntoStack(mainCam);
            }
        }
    }
}