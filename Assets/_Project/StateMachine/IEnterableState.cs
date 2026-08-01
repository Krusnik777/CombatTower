namespace StateMachine
{
    public interface IEnterableState : IState
    {
        public void Enter();
    }

    public interface IEnterableState<T> : IState
    {
        public void Enter(T enterParameters);
    }
}
