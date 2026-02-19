using System;
using UnityEngine;

namespace SoyoFramework.OptionalKits.StyledElementKit.Runtime
{
    [Serializable]
    public abstract class StyledElementHelperBase<TStyle>
        where TStyle : ElementStyle
    {
        // 变量
        [SerializeField] protected TStyle _style;

        public virtual TStyle Style
        {
            get => _style;
            set => _style = value;
        }
    }

    [Serializable]
    public class StyledElementHelper<TStyle> : StyledElementHelperBase<TStyle>
        where TStyle : ElementStyle
    {
        public override TStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    Update();
                }
            }
        }

        protected virtual void OnUpdate(TStyle style)
        {
        }

        public virtual void Update()
        {
            OnUpdate(Style);
        }
    }

    [Serializable]
    public class StyledElementHelper<TStyle, T> : StyledElementHelperBase<TStyle>
        where TStyle : ElementStyle
    {
        public override TStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    Update(default);
                }
            }
        }

        protected virtual void OnUpdate(TStyle style, in T para)
        {
        }

        public virtual void Update(in T para)
        {
            OnUpdate(Style, para);
        }
    }
}