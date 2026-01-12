using System;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.Framework.Runtime.Core.CoreUtils
{
    public interface IUnRegister
    {
        void UnRegister();
    }

    public class CustomUnRegister : IUnRegister
    {
        private Action _onUnRegister;

        public CustomUnRegister(Action onUnRegister)
        {
            _onUnRegister = onUnRegister;
        }

        public void UnRegister()
        {
            try
            {
                _onUnRegister.Invoke();
            }
            catch (Exception e)
            {
                $"取消注册时发生异常：{e}".LogError();
            }
            finally
            {
                _onUnRegister = null;
            }
        }
    }
}