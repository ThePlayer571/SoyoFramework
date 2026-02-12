using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;

namespace Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Models
{
    public interface IScoreModel : IModel
    {
        BindableProperty<int> Score { get; }
        BindableProperty<int> HighestScore { get; }
    }

    public class ScoreModel : AbstractModel, IScoreModel
    {
        public BindableProperty<int> Score { get; private set; }

        public BindableProperty<int> HighestScore { get; private set; }

        protected override void OnPreInit()
        {
            Score = new BindableProperty<int>(0);
            HighestScore = new BindableProperty<int>(0);
            Score.RegisterWithInitValue(score =>
            {
                if (score > HighestScore.Value)
                {
                    HighestScore.Value = score;
                }
            });
        }
    }
}