using System;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.UsefulTools;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.Examples.Exp01_Framework
{
    public class ScoreController : ViewController
    {
        [SerializeField] private Button AddBtn;
        [SerializeField] private Button SubBtn;
        [SerializeField] private Text ScoreText;

        private IProxy<IScoreModel> _ScoreModel;
        private IProxy<IScoreService> _ScoreService;

        private void Awake()
        {
            _ScoreModel = this.GetModel<IScoreModel>();
            _ScoreService = this.GetService<IScoreService>();

            // Input
            AddBtn.onClick.AddListener(() => { _ScoreService.Get.AddScore(); });

            SubBtn.onClick.AddListener(() => { _ScoreService.Get.SubScore(); });

            // 初始化 View
            ScoreText.text = _ScoreModel.Get.Score.ToString();

            // View 更新事件
            this.RegisterEvent<OnScoreChanged>(e => { ScoreText.text = e.NewScore.ToString(); });
        }
    }
}