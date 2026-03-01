using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.SoyoUGUIKit.Runtime
{
    /// <summary>
    /// 由于SelectionState是protected，不能写扩展😭，只能继承Selectable写转换函数
    /// </summary>
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