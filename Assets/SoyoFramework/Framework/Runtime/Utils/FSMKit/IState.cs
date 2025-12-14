namespace SoyoFramework.Framework.Runtime.Utils.FSMKit
{
    public interface IState<TStateId> where TStateId : notnull
    {
        TStateId StateId { get; }
        
        internal void OnUpdate();
        internal void OnEnter();
        internal void OnExit();
        internal void OnFixedUpdate();
        internal void OnLateUpdate();

        internal void OnAddedToFSM(FSM<TStateId> fsm);
    }
}