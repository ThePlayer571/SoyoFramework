using UnityEngine;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.StyleKit
{
    public abstract class ElementStyle : ScriptableObject
    {
        [SerializeField] private string _styleKey;
        
        public string StyleKey => _styleKey;
    }
}