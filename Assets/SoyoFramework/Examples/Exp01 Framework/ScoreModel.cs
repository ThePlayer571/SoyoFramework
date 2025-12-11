using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public interface IScoreModel : IModel
    {
        int Score { get; set; }
    }

    public class ScoreModel : AbstractModel, IScoreModel
    {
        private int _score = 0;

        // 此处更推荐使用BindableProperty<int>来简化代码，此处是为了演示事件系统和初学者友好
        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    this.SendEvent<OnScoreChanged>(new OnScoreChanged { NewScore = _score });
                }
            }
        }
    }
}