using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace Examples.UIKitExample.Scripts.MainGame
{
    public interface IMainModel : IModel
    {
        BindableProperty<string> UserName { get; }

        void Save();
    }

    public class MainModel : AbstractModel, IMainModel
    {
        private const string UserNameKey = "UIKitExample_UserName";

        public BindableProperty<string> UserName { get; private set; }

        public void Save()
        {
            PlayerPrefs.SetString(UserNameKey, UserName.Value);
        }

        protected override void OnPreInit()
        {
            UserName = new BindableProperty<string>(PlayerPrefs.GetString(UserNameKey, "Player"));
        }
    }
}