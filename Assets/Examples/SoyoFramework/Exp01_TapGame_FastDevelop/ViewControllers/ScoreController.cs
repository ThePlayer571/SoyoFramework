using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Commands;
using Examples.SoyoFramework.Exp01_TapGame_FastDevelop.Models;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.DefaultSyntacticSugar;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.SoyoFramework.Exp01_TapGame_FastDevelop.ViewControllers
{
    public class ScoreController : MonoBehaviour, IDefaultMonoVController
    {
        [SerializeField] private Button AddBtn;
        [SerializeField] private Button SubBtn;
        [SerializeField] private Text ScoreText;

        private IScoreModel _ScoreModel;
        private IUnRegister _scoreUnRegister;

        private void Awake()
        {
            _ScoreModel = this.GetModel<IScoreModel>();

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
    }
}