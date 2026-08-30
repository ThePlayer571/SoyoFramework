using System;
using System.Collections.Generic;
using SoyoFramework.Utils;
using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Backend.Aggregates
{
    public class Snake : AggregateMember<TheSnakeGame>
    {
        public List<Vector2Int> Body { get; private set; } = new();
        public EasyEvent AfterBodyChanged { get; private set; } = new();

        public void Move(Vector2Int direction)
        {
            direction = new Vector2Int(Math.Sign(direction.x), Math.Sign(direction.y)); // 归一化方向

            // 头部新位置
            var newHeadPosition = Body[0] + direction;

            if (newHeadPosition == Root.Map.Food.Position.Value)
            {
                // 吃到食物，蛇身增长
                Body.Insert(0, newHeadPosition);
                Root.Map.Food.Eat();
            }
            else
            {
                // 移动蛇身
                for (int i = Body.Count - 1; i > 0; i--)
                {
                    Body[i] = Body[i - 1];
                }

                Body[0] = newHeadPosition;
            }

            AfterBodyChanged.Trigger();
        }

        public Snake(TheSnakeGame root) : base(root)
        {
            Body.Add(new Vector2Int(5, 5));
            Body.Add(new Vector2Int(5, 4));
            Body.Add(new Vector2Int(5, 3));
        }
    }
}