using Examples.SoyoFramework.Exp02_TapGame.Models;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Tools;
using SoyoFramework.Framework.Runtime.Utils;

namespace Examples.SoyoFramework.Exp02_TapGame.ViewModels
{
    public interface IScoreViewModel : IViewModel
    {
        public IReadOnlyBindableProperty<int> Score { get; }
        public IReadOnlyBindableProperty<int> HighestScore { get; }
    }

    public class ScoreViewModel : AbstractViewModel, IScoreViewModel
    {
        public IReadOnlyBindableProperty<int> Score => _ScoreModel.Score;
        public IReadOnlyBindableProperty<int> HighestScore => _ScoreModel.HighestScore;

        private IScoreModel _ScoreModel;

        protected override void OnInit()
        {
            _ScoreModel = this.GetModel<IScoreModel>();
        }
    }
}