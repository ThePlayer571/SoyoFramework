using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    [RequireComponent(typeof(Camera))]
    public class UICameraBinder : MonoBehaviour
    {
        private void Start()
        {
            var mainCamera = GetComponent<Camera>();
            UIKit.PutUICameraIntoStack(mainCamera);
        }
    }
}