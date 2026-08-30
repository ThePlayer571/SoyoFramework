using System;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.Samples.SnakeGame.Backend.Aggregates
{
    public class ScoreBoard : AggregateMember<TheSnakeGame>
    {
        public int Score { get; private set; }

        public void AddScore(int value)
        {
            if (value <= 0)
            {
                new ArgumentException($"{nameof(value)} 必须是正数", nameof(value)).LogError();
                return;
            }
            
            Score += value;
        }

        public void Reset()
        {
            Score = 0;
        }

        public ScoreBoard(TheSnakeGame root) : base(root)
        {
        }
    }
}