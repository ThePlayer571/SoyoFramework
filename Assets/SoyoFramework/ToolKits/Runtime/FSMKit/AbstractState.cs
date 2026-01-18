namespace SoyoFramework.ToolKits.Runtime.FSMKit
{
    /// <summary>
    /// 推荐为所有State的基类
    /// </summary>
    /// <typeparam name="TStateId">该State的唯一标识符的类型</typeparam>
    public abstract class AbstractState<TStateId> : IState<TStateId>
    {
        #region IState接口

        public abstract TStateId StateId { get; }

        void IState<TStateId>.OnEnter()
        {
            OnEnter();
        }

        void IState<TStateId>.OnUpdate()
        {
            OnUpdate();
        }

        void IState<TStateId>.OnExit()
        {
            OnExit();
        }

        void IState<TStateId>.OnFixedUpdate()
        {
            OnFixedUpdate();
        }

        void IState<TStateId>.OnLateUpdate()
        {
            OnLateUpdate();
        }

        void IState<TStateId>.OnAddedToFSM(FSM<TStateId> fsm)
        {
            AttachedFSM = fsm;
        }

        #endregion

        #region 构造函数

        protected AbstractState()
        {
        }

        #endregion

        #region Protected

        protected FSM<TStateId> AttachedFSM { get; private set; }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnFixedUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }

        #endregion
    }

    /// <summary>
    /// 推荐为所有需要绑定对象的State的基类
    /// </summary>
    /// <typeparam name="TStateId">该State的唯一标识符的类型</typeparam>
    /// <typeparam name="TTarget">该State绑定的对象的类型</typeparam>
    public abstract class AbstractState<TStateId, TTarget> : AbstractState<TStateId>
    {
        protected TTarget AttachedTarget { get; private set; }

        protected AbstractState(TTarget target) : base()
        {
            AttachedTarget = target;
        }
    }
}