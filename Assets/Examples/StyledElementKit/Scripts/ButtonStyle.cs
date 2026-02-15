using SoyoFramework.OptionalKits.StyledElementKit.Runtime;
using UnityEngine;

namespace Examples.StyledElementKit.Scripts
{
    [CreateAssetMenu(menuName = "Create ButtonStyle", fileName = "ButtonStyle", order = 0)]
    public class ButtonStyle : ElementStyle
    {
        [SerializeField] private string _text;
        [SerializeField] private Color _pressedColor;

        public string Text => _text;
        public Color PressedColor => _pressedColor;
    }
}