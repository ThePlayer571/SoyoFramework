using SoyoFramework.Framework.Runtime.Core;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class UITapCommand : ICommand
    {
        public IArchitecture AttachedArchitecture { get; set; }
        public void Execute()
        {
        }
    }
    
    public class UIPauseCommand : ICommand
    {
        public IArchitecture AttachedArchitecture { get; set; }
        public void Execute()
        {
        }
    }
}