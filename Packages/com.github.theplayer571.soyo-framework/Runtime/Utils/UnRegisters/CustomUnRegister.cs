using System;
using SoyoFramework.Utils.LogKit;

namespace SoyoFramework.Utils.UnRegisters
{
    public class CustomUnRegister : IUnRegister
    {
        private Action? _onUnRegister;

        public CustomUnRegister(Action onUnRegister)
        {
            _onUnRegister = onUnRegister;
        }

        public void UnRegister()
        {
            var action = _onUnRegister;
            if (action == null) return;
            
            _onUnRegister = null;

            // 多播委托逐个执行：一个失败不影响其他
            foreach (var d in action.GetInvocationList())
            {
                try
                {
                    var a = d as Action;
                    a?.Invoke();
                }
                catch (Exception e)
                {
                    $"取消注册时发生异常：{e}".LogError();
                }
            }
        }
    }
}