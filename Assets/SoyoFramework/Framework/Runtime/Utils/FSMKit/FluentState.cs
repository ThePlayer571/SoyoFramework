using System;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.Framework.Runtime.Utils.FSMKit
{
    public class FluentState<TStateId> : AbstractState<TStateId>
    {
        internal FluentState(TStateId stateId)
        {
            StateId = stateId;
        }

        public override TStateId StateId { get; }

        private Action _onEnter;
        private Action _onUpdate;
        private Action _onExit;
        private Action _onFixedUpdate;
        private Action _onLateUpdate;

        public FluentState<TStateId> WithOnEnter(Action onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public FluentState<TStateId> WithOnUpdate(Action onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public FluentState<TStateId> WithOnExit(Action onExit)
        {
            _onExit += onExit;
            return this;
        }

        public FluentState<TStateId> WithOnFixedUpdate(Action onFixedUpdate)
        {
            _onFixedUpdate += onFixedUpdate;
            return this;
        }

        public FluentState<TStateId> WithOnLateUpdate(Action onLateUpdate)
        {
            _onLateUpdate += onLateUpdate;
            return this;
        }

        protected override void OnEnter()
        {
            _onEnter?.Invoke();
        }

        protected override void OnUpdate()
        {
            _onUpdate?.Invoke();
        }

        protected override void OnExit()
        {
            _onExit?.Invoke();
        }

        protected override void OnFixedUpdate()
        {
            _onFixedUpdate?.Invoke();
        }

        protected override void OnLateUpdate()
        {
            _onLateUpdate?.Invoke();
        }
    }

    public static class FluentStateExtension
    {
        public static FluentState<TStateId> AddFluentState<TStateId>(this FSM<TStateId> self, TStateId stateId)
        {
            var state = new FluentState<TStateId>(stateId);
            self.AddState(state);
            return state;
        }
        
        public static FluentState<TStateId> GetFluentState<TStateId>(this FSM<TStateId> self, TStateId stateId)
        {
            var state = self.GetState(stateId) as FluentState<TStateId>;
            if (state == null)
            {
                "找不到对应的FluentState: {stateId} ".LogError();
            }
            return state;
        }
    }
}