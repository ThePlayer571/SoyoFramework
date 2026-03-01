using SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.StyleKit;
using UnityEngine;

namespace Examples.StyledElementKit.Scripts
{
    [CreateAssetMenu(menuName = "Create ImageStyle", fileName = "ImageStyle", order = 0)]
    public class ImageStyle : ElementStyle
    {
        [SerializeField] private Color _bgColor;

        public Color BgColor => _bgColor;
    }
}