using System.Collections.Generic;
using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Backend.Aggregates
{
    public class Map : AggregateMember<TheSnakeGame>
    {
        // IReadOnlySet 更好（不暴露修改功能）（但是unity的C#版本太低了，没有这个接口）
        public ISet<Vector2Int> Walls => _wallSet;
        
        public Vector2Int Size {get; private set;}

        public Snake Snake { get; private set; }
        public Food Food { get; private set; }
        
        
        private readonly HashSet<Vector2Int> _wallSet = new HashSet<Vector2Int>();
        
        public Map(TheSnakeGame root,Vector2Int mapSize) : base(root)
        {
            Size = mapSize;
            Snake = new Snake(root);
            Food = new Food(root);
            for (int x = 0; x < mapSize.x; x++)
            {
                _wallSet.Add(new Vector2Int(x, 0));
                _wallSet.Add(new Vector2Int(x, mapSize.y - 1));
            }
            for (int y = 1; y < mapSize.y - 1; y++)
            {
                _wallSet.Add(new Vector2Int(0, y));
                _wallSet.Add(new Vector2Int(mapSize.x - 1, y));
            }
        }
    }
}