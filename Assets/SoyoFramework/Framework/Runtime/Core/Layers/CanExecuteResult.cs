using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public readonly struct CanExecuteResult
    {
        #region Core

        public readonly bool CanExecute;
        public readonly string FailMessage;

        #endregion

        #region 重载 && 隐式转换

        public static implicit operator bool(CanExecuteResult result) => result.CanExecute;

        public static implicit operator CanExecuteResult(bool canExecute) => new CanExecuteResult(canExecute);

        #endregion


        #region 构建

        public CanExecuteResult(bool canExecute, string failMessage)
        {
            CanExecute = canExecute;
            FailMessage = failMessage;
        }

        public CanExecuteResult(bool canExecute)
        {
            CanExecute = canExecute;
            FailMessage = null;
        }

        public static CanExecuteResult Success => new CanExecuteResult(true);
        public static CanExecuteResult Fail => new CanExecuteResult(false);
        public static CanExecuteResult FailReason(string message) => new CanExecuteResult(false, message);

        #endregion
    }
}