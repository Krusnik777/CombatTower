using System.Collections.Generic;

namespace StateMachine
{
    public abstract class AbstractStateMachine : IStateMachine, System.IDisposable
    {
        protected Dictionary<System.Type, IState> _states;

        protected IState _currentState;
        protected IState _previousState;

        public void Dispose()
        {
            _currentState?.Exit();
            _currentState = null;
        }

        public void SetState<T>() where T : class, IEnterableState
        {
            var state = HandleStateChange<T>();
            state.Enter();
        }

        public void SetState<T, P>(P enterParameters) where T : class, IEnterableState<P>
        {
            var state = HandleStateChange<T>();
            state.Enter(enterParameters);
        }

        protected virtual T HandleStateChange<T>() where T : class, IState
        {
            _currentState?.Exit();
            _previousState = _currentState;

            var state = _states[typeof(T)] as T;
            _currentState = state;

            return state;
        }
    }
}
