using System;

namespace SoyoFramework.OptionalKits.ActionKit.Runtime
{
    public class AutoUpdater
    {
        private Action _onEnter;
        private Action _onUpdate;
        private Action _onExit;

        public void StartUpdate()
        {
            _onEnter?.Invoke();
            ActionKit.RunOnUpdate(_onUpdate);
        }

        public void StopUpdate()
        {
            ActionKit.StopRunOnUpdate(_onUpdate);
            _onExit?.Invoke();
        }

        public AutoUpdater()
        {
        }

        public AutoUpdater WithOnEnter(Action onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public AutoUpdater WithOnUpdate(Action onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public AutoUpdater WithOnExit(Action onExit)
        {
            _onExit += onExit;
            return this;
        }
    }
}