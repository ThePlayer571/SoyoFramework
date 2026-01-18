using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.Framework.Runtime.Utils.LogKit.Interfaces;

namespace SoyoFramework.ToolKits.Runtime.FSMKit
{
    public interface IFSM<TStateId> where TStateId : notnull
    {
        // 状态管理
        void AddState(IState<TStateId> state);
        void ChangeState(TStateId stateId);
        bool TryChangeState(TStateId stateId);
        void StartWithState(TStateId stateId);

        // 生命周期
        void Update();
        void FixedUpdate();
        void LateUpdate();

        // 实用
        IState<TStateId> CurrentState { get; }
        IState<TStateId> PreviousState { get; }
        IState<TStateId> GetState(TStateId stateId);
        IReadOnlyCollection<TStateId> GetAllStateIds();
        EasyEvent<IState<TStateId>, IState<TStateId>> OnStateChanged { get; }
    }

    /// <summary>
    /// 有限状态机
    /// ”开始运行“的bool 等价于 CurrentState != null
    /// </summary>
    /// <typeparam name="TStateId"></typeparam>
    public class FSM<TStateId> : IFSM<TStateId>
    {
        #region IFSM接口实现

        public void AddState(IState<TStateId> state)
        {
            if (state == null)
            {
                "尝试添加一个空状态".LogError(_logger);
                return;
            }

            if (!_states.TryAdd(state.StateId, state))
            {
                $"已经包含状态: {state.StateId}，无法重复添加".LogError(_logger);
                return;
            }


            state.OnAddedToFSM(this);
        }

        public void ChangeState(TStateId stateId)
        {
            if (_currentState == null)
            {
                "FSM未开始运行".LogError(_logger);
                return;
            }

            ChangeStateInternal(stateId, true);
        }

        public bool TryChangeState(TStateId stateId)
        {
            if (_currentState == null)
                return false;
            return ChangeStateInternal(stateId, false);
        }

        public void StartWithState(TStateId stateId)
        {
            if (_currentState != null)
            {
                "FSM已经在运行".LogError(_logger);
                return;
            }

            ChangeStateInternal(stateId, true);
        }

        public void Update()
        {
            if (_currentState == null)
            {
                "FSM未开始运行".LogError(_logger);
                return;
            }

            try
            {
                _currentState.OnUpdate();
            }
            catch (Exception e)
            {
                $"状态 {_currentState.StateId} 的 OnUpdate 出现异常: {e}".LogError(_logger);
            }
        }

        public void FixedUpdate()
        {
            if (_currentState == null)
            {
                "FSM未开始运行".LogError(_logger);
                return;
            }

            try
            {
                _currentState.OnFixedUpdate();
            }
            catch (Exception e)
            {
                $"状态 {_currentState.StateId} 的 OnFixedUpdate 出现异常: {e}".LogError(_logger);
            }
        }

        public void LateUpdate()
        {
            if (_currentState == null)
            {
                "FSM未开始运行".LogError(_logger);
                return;
            }

            try
            {
                _currentState.OnLateUpdate();
            }
            catch (Exception e)
            {
                $"状态 {_currentState.StateId} 的 OnLateUpdate 出现异常: {e}".LogError(_logger);
            }
        }

        public IState<TStateId> CurrentState => _currentState;
        public TStateId CurrentStateId => CurrentState.StateId;
        public IState<TStateId> PreviousState => _previousState;

        public IState<TStateId> GetState(TStateId stateId)
        {
            if (_states.TryGetValue(stateId, out var state))
            {
                return state;
            }

            "找不到对应的状态: {stateId} ".LogError(_logger);
            return null;
        }

        public IReadOnlyCollection<TStateId> GetAllStateIds()
        {
            return _states.Keys;
        }

        public EasyEvent<IState<TStateId>, IState<TStateId>> OnStateChanged { get; } = new();

        #endregion

        private Dictionary<TStateId, IState<TStateId>> _states = new();
        private IState<TStateId> _currentState;
        private IState<TStateId> _previousState;
        private ILog _logger = new PrefixLogger("[FSM]", LogStrategy.WarningAndError);

        /// <summary>
        /// 切换状态，logOnFail控制是否输出日志，返回是否切换成功
        /// </summary>
        private bool ChangeStateInternal(TStateId newStateId, bool logOnFail)
        {
            if (!_states.TryGetValue(newStateId, out var newState))
            {
                if (logOnFail) $"找不到对应的状态: {newStateId} ".LogError(_logger);
                return false;
            }

            if (newState == _currentState)
            {
                if (logOnFail) "尝试切换到当前状态".LogError(_logger);
                return false;
            }

            var oldState = _currentState;
            _previousState = oldState;
            oldState?.OnExit();
            _currentState = newState;
            OnStateChanged.Trigger(oldState, newState);
            newState.OnEnter();
            return true;
        }
    }
}