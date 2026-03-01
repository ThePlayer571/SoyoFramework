using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime
{
    public abstract class SoyoToggle : Toggle
    {
        protected sealed override void DoStateTransition(SelectionState state, bool instant)
        {
            DoStateTransition(state, isOn, instant);
        }

        protected virtual void DoStateTransition(SelectionState state, bool isOn, bool instant)
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.RemoveListener(OnValueChanged);
            onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool _)
        {
            // 这里没有用回调的参数，因为如果其他脚本修改了 isOn 状态，参数会不准确
            // 例如：内置的MultiSelectGroup，订阅了onValueChanged，假设是onValueChanged(true)，然后它修改了isOn为false，此时会触发一个onValueChanged(false)，然后再触发一个onValueChanged(true)，导致状态不准确
            DoStateTransition(currentSelectionState, isOn, false);
        }
    }
}