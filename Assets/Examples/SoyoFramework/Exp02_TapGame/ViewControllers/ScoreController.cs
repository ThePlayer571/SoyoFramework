using System;
using Examples.SoyoFramework.Exp02_TapGame.Commands;
using Examples.SoyoFramework.Exp02_TapGame.ViewModels;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Tools;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.SoyoFramework.Exp02_TapGame.ViewControllers
{
    public class ScoreController : MonoBehaviour, IMonoVController
    {
        [SerializeField] private Button AddBtn;
        [SerializeField] private Button SubBtn;
        [SerializeField] private Text ScoreText;

        private IScoreViewModel _ScoreModel;
        private IUnRegister _scoreUnRegister;

        // 等待Awake执行架构初始化代码
        private void Start()
        {
            _ScoreModel = this.GetViewModel<IScoreViewModel>();

            // Input
            AddBtn.onClick.AddListener(() => { this.SendCommand(new AddScoreCommand()); });

            SubBtn.onClick.AddListener(() => { this.SendCommand(new SubScoreCommand()); });

            // Output
            _scoreUnRegister = _ScoreModel.Score.RegisterWithInitValue(newScore =>
            {
                ScoreText.text = newScore.ToString();
            });
        }

        private void OnDestroy()
        {
            AddBtn.onClick.RemoveAllListeners();
            SubBtn.onClick.RemoveAllListeners();
            _scoreUnRegister.UnRegister();
        }

        public IArchitecture RelyingArchitecture => TapGame.Instance;
    }
}