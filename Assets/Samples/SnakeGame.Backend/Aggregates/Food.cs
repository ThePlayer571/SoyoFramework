using System.Linq;
using SoyoFramework.Utils;
using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Backend.Aggregates
{
    public class Food : AggregateMember<TheSnakeGame>
    {
        public BindableProperty<Vector2Int> Position { get; }

        /// <summary>
        /// 食物被吃。
        /// </summary>
        public void Eat()
        {
            Root.ScoreBoard.AddScore(1);
            var availablePositions = (
                from x in Enumerable.Range(0, Root.Map.Size.x)
                from y in Enumerable.Range(0, Root.Map.Size.y)
                select new Vector2Int(x, y)
                into position
                where !Root.Map.Walls.Contains(position) && !Root.Map.Snake.Body.Contains(position)
                select position).ToList();

            if (availablePositions.Count == 0)
            {
                return;
            }
            else
            {
                Position.Value = availablePositions[Random.Range(0, availablePositions.Count)];
            }
        }

        public Food(TheSnakeGame root) : base(root)
        {
            Position = new BindableProperty<Vector2Int>(new Vector2Int(3, 3));
        }
    }
}