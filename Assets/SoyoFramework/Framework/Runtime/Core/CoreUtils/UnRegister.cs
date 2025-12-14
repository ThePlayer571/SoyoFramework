using System;

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
            _onUnRegister.Invoke();
            _onUnRegister = null;
        }
    }
}