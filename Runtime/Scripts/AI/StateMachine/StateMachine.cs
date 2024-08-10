using System;
using System.Collections.Generic;

namespace m039.Common.StateMachine
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

            for (int i = 0; i < _anyTransitions.Count; i++)
            {
                var transition = _anyTransitions[i];

                if (transition.condition())
                {
                    SetState(transition.to);
                    return;
                }
            }

            if (CurrentState != null && _transitions.ContainsKey(CurrentState))
            {
                var transitions = _transitions[CurrentState];
                for (int i = 0; i < transitions.Count; i++)
                {
                    var transition = transitions[i];
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
            if (from == null || to == null || condition == null)
                return;

            if (!_transitions.ContainsKey(from))
            {
                _transitions[from] = new List<Transition>();
            }

            _transitions[from].Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            if (to == null || condition == null)
                return;

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
