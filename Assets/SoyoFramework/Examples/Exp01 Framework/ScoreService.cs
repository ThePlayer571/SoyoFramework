using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public interface IScoreService : IService
    {
        void AddScore();
        void SubScore();
    }

    public class ScoreService : AbstractService, IScoreService
    {
        private IProxy<IScoreModel> _ScoreModel;

        public void AddScore()
        {
            _ScoreModel.Get.Score++;
        }

        public void SubScore()
        {
            _ScoreModel.Get.Score--;
        }

        protected override void OnInit()
        {
            _ScoreModel = this.GetModel<IScoreModel>();
        }
    }
}