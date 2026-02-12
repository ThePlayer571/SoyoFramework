using Examples.SoyoFramework.Exp02_TapGame.Models;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace Examples.SoyoFramework.Exp02_TapGame.Commands
{
    public class SubScoreCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var scoreModel = this.GetModel<IScoreModel>();
            scoreModel.Score.Value -= 1;
        }

        public override CanExecuteResult CanExecute()
        {
            var scoreModel = this.GetModel<IScoreModel>();
            
            if (scoreModel == null)
            {
                return CanExecuteResult.FailReason("找不到ScoreModel");
            }
            
            return CanExecuteResult.Success;
        }
    }
}