using SoyoFramework.Samples.SnakeGame.Backend.Aggregates;

namespace SoyoFramework.Samples.SnakeGame.Backend
{
    public class TSG : Architecture<TSG>
    {
        protected override void OnInit()
        {
            this.RegisterAggregateRoot(new TheSnakeGame());
        }

        protected override void OnDeinit()
        {
        }
    }
}
