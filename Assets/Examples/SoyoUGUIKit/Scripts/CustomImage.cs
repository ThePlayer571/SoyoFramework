using System;
using SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.StyleKit;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.StyledElementKit.Scripts
{
    public class CustomImage : MonoBehaviour
    {
        [SerializeField] private StyledImageHelper _styleHelper;

        [SerializeField] private Image _image;

        private void OnValidate()
        {
            _styleHelper.Update();
        }


        [Serializable]
        private class StyledImageHelper : StyledElementHelper<ImageStyle>
        {
            [SerializeField] private CustomImage _self;

            public StyledImageHelper(CustomImage self)
            {
                _self = self;
            }

            protected override void OnUpdate(ImageStyle style)
            {
                _self._image.color = style.BgColor;
            }
        }
    }
}