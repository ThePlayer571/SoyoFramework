using System;
using SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.StyleKit;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.StyledElementKit.Scripts
{
    public partial class CustomButton : Button
    {
        [SerializeField] private CustomButtonHelper _styledButtonHelper;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            _styledButtonHelper.Update(state);
        }
    }

    public partial class CustomButton
    {
        [Serializable]
        private class CustomButtonHelper : StyledElementHelper<ButtonStyle, SelectionState>
        {
            [SerializeField] private Text _text;
            [SerializeField] private Image _image;

            protected override void OnUpdate(ButtonStyle style, in SelectionState para)
            {
                _text.text = _style.Text;
                if (para == SelectionState.Pressed)
                {
                    _image.color = _style.PressedColor;
                }
                else
                {
                    _image.color = Color.white;
                }
            }
        }
    }
}