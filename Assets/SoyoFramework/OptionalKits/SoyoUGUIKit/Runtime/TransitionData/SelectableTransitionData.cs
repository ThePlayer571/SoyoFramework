using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.TransitionData
{
    /// <summary>
    /// 可复用的 Selectable 过渡配置数据类
    /// 包含 ColorTint、SpriteSwap、Animation 三种过渡类型的配置
    /// </summary>
    [Serializable]
    public class SelectableTransitionData
    {
        #region Serialized Fields

        [SerializeField] private Selectable.Transition m_Transition = Selectable.Transition.ColorTint;
        [SerializeField] private Graphic m_TargetGraphic;
        [SerializeField] private ColorBlock m_Colors = ColorBlock.defaultColorBlock;
        [SerializeField] private SpriteState m_SpriteState;
        [SerializeField] private AnimationTriggers m_AnimationTriggers = new AnimationTriggers();

        #endregion

        #region Properties

        /// <summary>
        /// 当前使用的过渡类型
        /// </summary>
        public Selectable.Transition transition
        {
            get => m_Transition;
            set => m_Transition = value;
        }

        /// <summary>
        /// 过渡效果的目标图形组件
        /// </summary>
        public Graphic targetGraphic
        {
            get => m_TargetGraphic;
            set => m_TargetGraphic = value;
        }

        /// <summary>
        /// 便捷属性：将 TargetGraphic 转换为 Image（用于 SpriteSwap）
        /// </summary>
        public Image image
        {
            get => m_TargetGraphic as Image;
            set => m_TargetGraphic = value;
        }

        /// <summary>
        /// 颜色过渡配置 (用于 ColorTint 模式)
        /// </summary>
        public ColorBlock colors
        {
            get => m_Colors;
            set => m_Colors = value;
        }

        /// <summary>
        /// 精灵状态配置 (用于 SpriteSwap 模式)
        /// </summary>
        public SpriteState spriteState
        {
            get => m_SpriteState;
            set => m_SpriteState = value;
        }

        /// <summary>
        /// 动画触发器配置 (用于 Animation 模式)
        /// </summary>
        public AnimationTriggers animationTriggers
        {
            get => m_AnimationTriggers;
            set => m_AnimationTriggers = value;
        }

        /// <summary>
        /// 获取颜色过渡的持续时间
        /// </summary>
        public float fadeDuration => m_Colors.fadeDuration;

        /// <summary>
        /// 获取颜色乘数
        /// </summary>
        public float colorMultiplier => m_Colors.colorMultiplier;

        /// <summary>
        /// 获取 TargetGraphic 所在 GameObject 上的 Animator 组件
        /// </summary>
        public Animator animator
        {
            get => m_TargetGraphic != null ? m_TargetGraphic.GetComponent<Animator>() : null;
        }

        #endregion

        #region State Getters

        /// <summary>
        /// 根据选择状态获取对应的颜色
        /// </summary>
        public Color GetStateColor(SoyoSelectionState state)
        {
            switch (state)
            {
                case SoyoSelectionState.Normal:
                    return m_Colors.normalColor;
                case SoyoSelectionState.Highlighted:
                    return m_Colors.highlightedColor;
                case SoyoSelectionState.Pressed:
                    return m_Colors.pressedColor;
                case SoyoSelectionState.Selected:
                    return m_Colors.selectedColor;
                case SoyoSelectionState.Disabled:
                    return m_Colors.disabledColor;
                default:
                    return Color.black;
            }
        }

        /// <summary>
        /// 根据选择状态获取对应的精灵图
        /// </summary>
        public Sprite GetStateSprite(SoyoSelectionState state)
        {
            switch (state)
            {
                case SoyoSelectionState.Normal:
                    return null;
                case SoyoSelectionState.Highlighted:
                    return m_SpriteState.highlightedSprite;
                case SoyoSelectionState.Pressed:
                    return m_SpriteState.pressedSprite;
                case SoyoSelectionState.Selected:
                    return m_SpriteState.selectedSprite;
                case SoyoSelectionState.Disabled:
                    return m_SpriteState.disabledSprite;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 根据选择状态获取对应的动画触发器名称
        /// </summary>
        public string GetStateTrigger(SoyoSelectionState state)
        {
            switch (state)
            {
                case SoyoSelectionState.Normal:
                    return m_AnimationTriggers.normalTrigger;
                case SoyoSelectionState.Highlighted:
                    return m_AnimationTriggers.highlightedTrigger;
                case SoyoSelectionState.Pressed:
                    return m_AnimationTriggers.pressedTrigger;
                case SoyoSelectionState.Selected:
                    return m_AnimationTriggers.selectedTrigger;
                case SoyoSelectionState.Disabled:
                    return m_AnimationTriggers.disabledTrigger;
                default:
                    return string.Empty;
            }
        }

        #endregion

        #region Static & Utility

        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static SelectableTransitionData Default => new SelectableTransitionData();

        /// <summary>
        /// 验证数据有效性（通常在 OnValidate 中调用）
        /// </summary>
        public void Validate()
        {
            if (m_Colors.fadeDuration < 0f)
            {
                var colors = m_Colors;
                colors.fadeDuration = 0f;
                m_Colors = colors;
            }
        }

        #endregion
    }
}