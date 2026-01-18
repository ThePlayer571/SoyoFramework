using SoyoFramework.Framework.Examples.Exp01_TapGame.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.Framework.Examples.Exp01_TapGame
{
    public interface IScoreModel : IModel
    {
        // 这里展示两种发送事件的方式，实践中更推荐都使用 BindableProperty
        int Score { get; set; }
        BindableProperty<int> HighestScore { get; set; }
    }

    public class ScoreModel : AbstractModel, IScoreModel
    {
        private int _Score;

        public int Score
        {
            get => _Score;
            set
            {
                if (_Score == value)
                    return;

                _Score = value;
                this.SendEvent(new AfterScoreChanged { Score = _Score });
                if (_Score > HighestScore.Value)
                {
                    HighestScore.Value = _Score;
                }
            }
        }

        public BindableProperty<int> HighestScore { get; set; }

        protected override void OnPreInit()
        {
            Score = 0;
            HighestScore = new BindableProperty<int>(0);
        }
    }
}