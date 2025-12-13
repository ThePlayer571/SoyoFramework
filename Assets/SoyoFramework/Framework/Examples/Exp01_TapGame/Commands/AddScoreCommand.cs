using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Examples.Exp01_TapGame.Commands
{
    public class AddScoreCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var scoreModel = this.GetModel<IScoreModel>();
            scoreModel.Score += 1;
        }
    }
}