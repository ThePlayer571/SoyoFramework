using UnityEngine;
using UnityEngine.UI;

namespace Examples.StyledElementKit.Scripts
{
    public class CustomButton2 : Button
    {
        [SerializeField] private ImageStyle _style;
        [SerializeField] private Image _image;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            _image.color = _style.BgColor;
        }
    }
}