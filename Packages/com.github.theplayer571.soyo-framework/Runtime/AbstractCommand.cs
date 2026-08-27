using System;

namespace SoyoFramework
{
    public abstract class AbstractCommand : ICommand
    {
        protected abstract void OnExecute();

        void ICommand.Execute()
        {
            OnExecute();
        }

        public virtual CanExecuteResult CanExecute()
        {
            return CanExecuteResult.Success;
        }

        private IArchitecture? _architecture;

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture
        {
            get => _architecture ?? throw new InvalidOperationException(
                $"尝试在 {nameof(ICommand)} 初始化前访问 {nameof(ICanAttachToArchitecture.AttachedArchitecture)}。请先调用 {nameof(IArchitecture)}.{nameof(IArchitecture.InitCommand)} 来初始化 {nameof(ICommand)}，或者通过 {nameof(IArchitecture)}.{nameof(IArchitecture.SendCommand)} 直接发送命令");
            set => _architecture = value;
        }
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        protected abstract TResult OnExecute();

        TResult ICommand<TResult>.Execute()
        {
            return OnExecute();
        }

        void ICommand.Execute()
        {
            ((ICommand<TResult>)this).Execute();
        }

        public virtual CanExecuteResult CanExecute()
        {
            return CanExecuteResult.Success;
        }

        private IArchitecture? _architecture;

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture
        {
            get => _architecture ?? throw new InvalidOperationException(
                $"尝试在 {nameof(ICommand)} 初始化前访问 {nameof(ICanAttachToArchitecture.AttachedArchitecture)}。请先调用 {nameof(IArchitecture)}.{nameof(IArchitecture.InitCommand)} 来初始化 {nameof(ICommand)}，或者通过 {nameof(IArchitecture)}.{nameof(IArchitecture.SendCommand)} 直接发送命令");
            set => _architecture = value;
        }
    }
}