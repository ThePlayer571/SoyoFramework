using UnityEngine;

namespace SoyoFramework.OptionalKits.StyledElementKit.Runtime
{
    public abstract class ElementStyle : ScriptableObject
    {
        [SerializeField] private string _styleKey;
        
        public string StyleKey => _styleKey;
    }
}