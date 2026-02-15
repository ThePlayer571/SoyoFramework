using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace Examples.UIKitExample.Scripts.TapGame
{
    public class AddScoreCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<ITapGameModel>();
            if (model.IsInWildMode)
            {
                model.Score.Value += 2;
            }
            else
            {
                model.Score.Value++;
            }
        }
    }
}