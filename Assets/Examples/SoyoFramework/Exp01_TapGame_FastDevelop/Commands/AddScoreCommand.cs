using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Models;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Commands
{
    public class AddScoreCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var scoreModel = this.GetModel<IScoreModel>();
            scoreModel.Score.Value += 1;
        }
    }
}