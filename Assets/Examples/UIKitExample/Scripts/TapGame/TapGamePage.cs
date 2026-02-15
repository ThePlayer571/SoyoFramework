using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Examples.UIKitExample.Scripts.MainGame;
using Examples.UIKitExample.Scripts.SettingsPanel;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.OptionalKits.UIKit.Runtime.Page;
using SoyoFramework.ToolKits.Runtime.FluentAPI;
using Random = UnityEngine.Random;

namespace Examples.UIKitExample.Scripts.TapGame
{
    public class TapGamePage : UIPage
    {
        // 引用
        private ITapGameModel _TapGameModel;
        private IMainModel _MainModel;

        private TapGameContext _TapGameContext;

        protected override void Configure()
        {
            _TapGameContext = new TapGameContext();
            RegisterContext(_TapGameContext);
            RegisterContext<ISettingsContext>(new SettingsContext());
        }

        protected override void OnInit()
        {
            _TapGameModel = this.GetModel<ITapGameModel>();
            _MainModel = this.GetModel<IMainModel>();

            _TapGameModel.Score.RegisterWithInitValue(score =>
            {
                //
                _TapGameContext.Score.Value = score;
            }).UnRegisterWhenGameObjectDestroyed(this);

            RegisterLogic(new SettingsLogic(this, _MainModel));
        }

        protected override void OnClose()
        {
        }

        protected override bool HandleUICommand(UICommand command)
        {
            return false;
        }

        protected override bool HandleUICommand<TResult>(UICommand<TResult> command, out TResult result)
        {
            switch (command)
            {
                // bad: 把狂野模式的逻辑放在了 UI 层
                case EnterWildModeCommand:
                    var task = UniTask.Create(async () =>
                    {
                        await UniTask.WaitForSeconds(2f);
                        var isSuccess = Random.value > 0.5f;
                        if (isSuccess)
                        {
                            _TapGameModel.IsInWildMode = true;
                        }

                        return isSuccess;
                    });

                    result = (TResult)(object)task;
                    return true;
            }

            result = default;
            return false;
        }

        public override IArchitecture RelyingArchitecture => TwoGames.Instance;
    }
}