using System.Collections.Generic;
using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Backend.Events
{
    public class AfterSnakeBodyChanged
    {
        public List<Vector2Int> NewBody { get; }
        
        public AfterSnakeBodyChanged(List<Vector2Int> newBody)
        {
            NewBody = newBody;
        }
    }
}