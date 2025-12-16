namespace SoyoFramework.Scripts.ToolKits.UIKit
{
    public interface IUIPanel
    {
    }
    
    public interface IUIMainPanel : IUIPanel
    {
        void Init(object initData);
        void Close();
    }
    
    public interface IStackablePanel
    {
        void OnPushed() {}
        void OnPopped();
    }
}