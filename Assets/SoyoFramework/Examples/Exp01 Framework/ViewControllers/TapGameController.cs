using SoyoFramework.Examples.Exp01_Framework.Events;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.Examples.Exp01_Framework.ViewControllers
{
    public class ScoreController : MonoVController
    {
        [SerializeField] private Button AddBtn;
        [SerializeField] private Button SubBtn;
        [SerializeField] private Text ScoreText;

        private IScoreModel _ScoreModel;

        // 用Awake会导致框架没初始化（因为GameInitializer是Awake调用的，这个问题在正式项目里肯定不会出现）
        private void Start()
        {
            _ScoreModel = this.GetModel<IScoreModel>();

            // 涉及场景切换的话，记得取消注册哦
            // Input
            AddBtn.onClick.AddListener(() => { this.SendCommand(new AddScoreCommand()); });

            SubBtn.onClick.AddListener(() => { this.SendCommand(new SubScoreCommand()); });

            // 初始化 View
            ScoreText.text = _ScoreModel.Score.ToString();

            // 订阅 Model 变化
            this.RegisterEvent<AfterScoreChanged>(e => { ScoreText.text = e.Score.ToString(); });
        }
    }
}