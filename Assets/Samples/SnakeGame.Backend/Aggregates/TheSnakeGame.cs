using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Backend.Aggregates
{
    // 本来该叫SnakeGame的，但是与命名空间命名冲突了
    public class TheSnakeGame : AggregateRoot
    {
        public Map Map { get; private set; }
        public ScoreBoard ScoreBoard { get; private set; }

        public TheSnakeGame()
        {
            Map = new Map(this, new Vector2Int(10, 10));
            ScoreBoard = new ScoreBoard(this);
        }

        protected override void OnUnregistered()
        {
        }
    }
}
