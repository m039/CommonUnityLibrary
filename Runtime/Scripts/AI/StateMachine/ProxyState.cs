using System;

namespace m039.Common.StateMachine
{

    /// <summary>
    /// This class allowes to swap a state on the fly in the Inspector.
    /// </summary>
    public class ProxyState : IState
    {
        readonly Func<IState> _provider;

        public IState State => _provider.Invoke();

        public ProxyState(Func<IState> provider)
        {
            _provider = provider;
        }

        public virtual void OnEnter()
        {
            _provider()?.OnEnter();
        }

        public virtual void OnExit()
        {
            _provider()?.OnExit();
        }

        public virtual void OnFixedUpdate()
        {
            _provider()?.OnFixedUpdate();
        }

        public virtual void OnUpdate()
        {
            _provider().OnUpdate();
        }
    }
}
