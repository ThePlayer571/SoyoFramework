using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Framework.Examples.Exp02_ComplexTapGame.Commands
{
    public class SubScoreCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var scoreModel = this.GetModel<IScoreModel>();
            scoreModel.Score -= 1;
        }
    }
}