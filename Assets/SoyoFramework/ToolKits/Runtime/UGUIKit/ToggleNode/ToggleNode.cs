namespace SoyoFramework.ToolKits.Runtime.UGUIKit.ToggleNode
{
    /// <summary>
    /// 使用 string 作为 Key 的 Toggle 引用容器
    /// </summary>
    public class ToggleNode : ToggleNodeBase<string>
    {
        protected override bool IsValidKey(string key)
        {
            return !string.IsNullOrEmpty(key);
        }
    }
}