using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.ToolKits.Runtime.UGUIKit
{
    public class BetterToggle : Toggle
    {
        public Color isOnColor = Color.white;
        public Sprite isOnSprite;
        public string isOnTrigger = "On";

        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(OnToggleValueChanged);
            RefreshView();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool value)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            DoStateTransition(currentSelectionState, false);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (isOn && targetGraphic != null)
            {
                switch (transition)
                {
                    case Transition.ColorTint:
                        targetGraphic.CrossFadeColor(isOnColor, instant ? 0f : colors.fadeDuration, true, true);
                        break;
                    case Transition.SpriteSwap:
                        var img = targetGraphic as Image;
                        if (img != null)
                            img.overrideSprite = isOnSprite;
                        break;
                    case Transition.Animation:
#if PACKAGE_ANIMATION
                        var animator = targetGraphic. GetComponent<Animator>();
                        if (animator != null && animator.isActiveAndEnabled && ! string.IsNullOrEmpty(isOnTrigger))
                        {
                            animator.SetTrigger(isOnTrigger);
                        }
#endif
                        break;
                    default:
                        base.DoStateTransition(state, instant);
                        break;
                }
            }
            else
            {
                base.DoStateTransition(state, instant);
            }
        }
    }
}