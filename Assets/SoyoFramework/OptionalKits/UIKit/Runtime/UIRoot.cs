using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    public class UIRoot : MonoBehaviour
    {
        public Camera UICamera;
        public Canvas Canvas;
        [SerializeField] internal UIManager UIManager;
    }
}