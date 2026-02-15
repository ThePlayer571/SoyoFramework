using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Layers;

namespace Examples.UIKitExample.Scripts.MainGame
{
    public class SetUserNameCommand : AbstractCommand
    {
        public SetUserNameCommand(string userName)
        {
            _userName = userName;
        }

        private string _userName;

        protected override void OnExecute()
        {
            var mainModel = this.GetModel<IMainModel>();
            mainModel.UserName.Value = _userName;
            mainModel.Save();
        }
    }
}