using UnityEngine;

namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    public class UIRoot : MonoBehaviour
    {
        internal static UIRoot Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public RectTransform Common;
        public RectTransform PopUI;
        public RectTransform Transition;
        public Camera UICamera;
        public Canvas UICanvas;

        [SerializeField] internal UIManager UIManager;
    }
}