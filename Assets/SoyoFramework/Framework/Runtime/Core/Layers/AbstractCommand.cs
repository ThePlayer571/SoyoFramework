using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class AbstractCommand : ICommand
    {
        protected abstract void OnExecute();

        void ICommand.Execute(bool ignoreCanExecuteCheck)
        {
            if (ignoreCanExecuteCheck)
            {
                OnExecute();
                return;
            }

            var canExecuteResult = CanExecute();

            if (canExecuteResult.CanExecute)
            {
                OnExecute();
            }
            else
            {
                Debug.LogError($"Command执行失败：{GetType().FullName}, 原因：{canExecuteResult.FailMessage}");
            }
        }

        public virtual CanExecuteResult CanExecute()
        {
            return CanExecuteResult.Success;
        }

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        protected abstract TResult OnExecute();

        TResult ICommand<TResult>.Execute(bool ignoreCanExecuteCheck)
        {
            if (ignoreCanExecuteCheck)
            {
                return OnExecute();
            }

            var canExecuteResult = CanExecute();
            if (canExecuteResult.CanExecute)
            {
                return OnExecute();
            }
            else
            {
                Debug.LogError($"Command执行失败：{GetType().FullName}， 原因：{canExecuteResult.FailMessage}");
                return default;
            }
        }

        public virtual CanExecuteResult CanExecute()
        {
            return CanExecuteResult.Success;
        }

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
    }
}