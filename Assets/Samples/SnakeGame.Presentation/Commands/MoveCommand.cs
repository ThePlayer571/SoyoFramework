using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;
using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Presentation.Commands
{
    public class MoveCommand : AbstractCommand
    {
        public MoveCommand(Vector2Int direction)
        {
            _direction = direction;
        }
        
        private readonly Vector2Int _direction;
        
        protected override void OnExecute()
        {
            var game = this.GetAggregateRoot<TheSnakeGame>();
            game!.Map.Snake.Move(_direction);
        }

        public override CanExecuteResult CanExecute()
        {
            var game = this.GetAggregateRoot<TheSnakeGame>();
            if (game == null)
            {
                return "缺少依赖：TheSnakeGame";
            }

            return true;
        }
    }
}