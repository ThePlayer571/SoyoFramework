using UnityEngine.UI;

namespace SoyoFramework.SoyoUGUIKit.Runtime
{
    public abstract class SoyoSelectable : Selectable
    {
        protected SoyoSelectionState ToSoyoSelectionState(SelectionState state)
        {
            return state switch
            {
                SelectionState.Normal => SoyoSelectionState.Normal,
                SelectionState.Highlighted => SoyoSelectionState.Highlighted,
                SelectionState.Pressed => SoyoSelectionState.Pressed,
                SelectionState.Selected => SoyoSelectionState.Selected,
                SelectionState.Disabled => SoyoSelectionState.Disabled,
                _ => SoyoSelectionState.Normal,
            };
        }
    }
}