using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class AbstractCommand : ICommand
    {
        protected abstract void OnExecute();

        void ICommand.Execute()
        {
            if (CanExecute())
            {
                OnExecute();
            }
            else
            {
                Debug.LogError($"Command执行失败：{GetType().FullName}");
            }
        }

        public virtual bool CanExecute() => true;

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        protected abstract TResult OnExecute();

        TResult ICommand<TResult>.Execute()
        {
            if (CanExecute())
            {
                return OnExecute();
            }
            else
            {
                Debug.LogError($"Command执行失败：{GetType().FullName}");
                return default;
            }
        }

        public virtual bool CanExecute() => true;

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
    }
}