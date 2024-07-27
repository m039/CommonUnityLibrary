using System;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.StateMachine
{
    /// <summary>
    /// Use this class in a hierarchical state machine.
    /// </summary>
    public class MonoBehaviourState : MonoBehaviour, IState
    {
        public StateMachine StateMachine { get; private set; } = new();

        bool _onEnterCalled = false;

        IState _firstState = null;

        MonoBehaviourState _exitState = null;

        public void SetState(IState state)
        {
            if (!_onEnterCalled)
            {
                _firstState = state;
            }
            else
            {
                StateMachine.SetState(state);
            }
            _exitState = null;
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            StateMachine.AddTransition(from, to, condition);
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            StateMachine.AddAnyTransition(to, condition);
        }

        public virtual void OnEnter()
        {
            if (!_onEnterCalled)
            {
                StateMachine.SetState(_firstState);
                _firstState = null;
                _onEnterCalled = true;
            }

            // Resume previously stopped state.
            if (_exitState != null)
            {
                StateMachine.SetState(_exitState);
                _exitState = null;
            }
        }

        public virtual void OnExit()
        {
            if (StateMachine.CurrentState is MonoBehaviourState state)
            {
                state.OnExit();
                _exitState = state;
            }
        }

        public virtual void OnFixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        public virtual void OnUpdate()
        {
            StateMachine.Update();
        }

        public List<IState> GetHierarchicalStates()
        {
            return GetHierarchicalStatesInternal(null);
        }

        List<IState> GetHierarchicalStatesInternal(List<IState> states)
        {
            if (states == null)
            {
                states = new();
            }

            IState currentState;

            if (StateMachine.CurrentState is ProxyState proxyState)
            {
                currentState = proxyState.State;
            } else
            {
                currentState = StateMachine.CurrentState;
            }

            states.Add(this);
            if (currentState == null)
                return states;

            if (currentState is MonoBehaviourState state2)
            {
                return state2.GetHierarchicalStatesInternal(states);
            } else
            {
                states.Add(currentState);
            }

            return states;
        }
    }

}
