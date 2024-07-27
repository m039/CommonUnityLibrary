using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace m039.Common
{
    public interface IState
    {
        void OnEnter();

        void OnExit();

        void OnUpdate();

        void OnFixedUpdate();
    }

    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        readonly Dictionary<IState, List<Transition>> _transitions = new();

        readonly List<Transition> _anyTransitions = new();

        public void Update()
        {
            CurrentState?.OnUpdate();

            foreach (var transition in _anyTransitions)
            {
                if (transition.condition())
                {
                    SetState(transition.to);
                    return;
                }
            }

            if (CurrentState != null)
            {
                foreach (var transition in _transitions[CurrentState])
                {
                    if (transition.condition())
                    {
                        SetState(transition.to);
                        return;
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }

        public void SetState(IState state)
        {
            if (state == CurrentState)
                return;

            CurrentState?.OnExit();
            CurrentState = state;
            CurrentState?.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            Assert.IsNotNull(from);
            Assert.IsNotNull(to);
            Assert.IsNotNull(condition);

            if (!_transitions.ContainsKey(from))
            {
                _transitions[from] = new List<Transition>();
            }

            _transitions[from].Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            Assert.IsNotNull(to);
            Assert.IsNotNull(condition);

            _anyTransitions.Add(new Transition(to, condition));
        }

        public readonly struct Transition
        {
            public readonly IState to;

            public readonly Func<bool> condition;

            public Transition(IState to, Func<bool> condition)
            {
                this.to = to;
                this.condition = condition;
            }
        }
    }
}
