using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;

namespace Examples.UIKitExample.Scripts.TapGame
{
    public interface ITapGameModel : IModel
    {
        BindableProperty<int> Score { get; }
        bool IsInWildMode { get; set; }
    }

    public class TapGameModel : AbstractModel, ITapGameModel
    {
        public BindableProperty<int> Score { get; } = new();
        public bool IsInWildMode { get; set; }
    }
}