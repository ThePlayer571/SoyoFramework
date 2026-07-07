using UnityEngine;

namespace SoyoFramework.SoyoUGUIKit.Runtime.StyleKit
{
    public abstract class ElementStyle : ScriptableObject
    {
        [SerializeField] private string _styleKey;
        
        public string StyleKey => _styleKey;
    }
}