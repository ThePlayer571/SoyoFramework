using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime.TransitionData
{
    /// <summary>
    /// SelectableTransitionData 的扩展方法
    /// 提供过渡效果的执行逻辑
    /// </summary>
    public static class SelectableTransitionDataExtensions
    {
        #region State Transition

        /// <summary>
        /// 执行状态过渡（自动根据 Transition 类型选择过渡方式）
        /// </summary>
        /// <param name="data">过渡数据</param>
        /// <param name="state">目标状态</param>
        /// <param name="instant">是否立即切换</param>
        public static void DoStateTransition(this SelectableTransitionData data, SoyoSelectionState state,
            bool instant = false)
        {
            if (data == null || data.targetGraphic == null)
                return;

            switch (data.transition)
            {
                case Selectable.Transition.ColorTint:
                    data.StartColorTween(state, instant);
                    break;
                case Selectable.Transition.SpriteSwap:
                    data.DoSpriteSwap(state);
                    break;
                case Selectable.Transition.Animation:
                    data.TriggerAnimation(state);
                    break;
                case Selectable.Transition.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// 清除状态（重置为默认/Normal状态）
        /// </summary>
        /// <param name="data">过渡数据</param>
        public static void InstantClearState(this SelectableTransitionData data)
        {
            if (data == null)
                return;

            switch (data.transition)
            {
                case Selectable.Transition.ColorTint:
                    data.targetGraphic?.CrossFadeColor(Color.white, 0f, true, true);
                    break;
                case Selectable.Transition.SpriteSwap:
                    if (data.image != null)
                        data.image.overrideSprite = null;
                    break;
                case Selectable.Transition.Animation:
                    data.TriggerAnimation(SoyoSelectionState.Normal);
                    break;
            }
        }

        #endregion

        #region ColorTint

        /// <summary>
        /// 执行颜色过渡
        /// </summary>
        private static void StartColorTween(this SelectableTransitionData data, SoyoSelectionState state, bool instant)
        {
            if (data.targetGraphic == null)
                return;

            Color targetColor = data.GetStateColor(state) * data.colorMultiplier;
            data.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : data.fadeDuration, true, true);
        }

        #endregion

        #region SpriteSwap

        /// <summary>
        /// 执行精灵切换
        /// </summary>
        private static void DoSpriteSwap(this SelectableTransitionData data, SoyoSelectionState state)
        {
            if (data.image == null)
                return;

            data.image.overrideSprite = data.GetStateSprite(state);
        }

        #endregion

        #region Animation

        /// <summary>
        /// 触发动画过渡
        /// </summary>
        private static void TriggerAnimation(this SelectableTransitionData data, SoyoSelectionState state)
        {
            var animator = data.animator;
            if (animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables)
                return;

            string triggerName = data.GetStateTrigger(state);
            if (string.IsNullOrEmpty(triggerName))
                return;

            // 重置所有触发器
            animator.ResetTrigger(data.animationTriggers.normalTrigger);
            animator.ResetTrigger(data.animationTriggers.highlightedTrigger);
            animator.ResetTrigger(data.animationTriggers.pressedTrigger);
            animator.ResetTrigger(data.animationTriggers.selectedTrigger);
            animator.ResetTrigger(data.animationTriggers.disabledTrigger);

            animator.SetTrigger(triggerName);
        }

        #endregion
    }
}