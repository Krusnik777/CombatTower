namespace StateMachine
{
    public interface IStateMachine
    {
        public void SetState<T>() where T : class, IEnterableState;
        public void SetState<T, P>(P enterParameters) where T : class, IEnterableState<P>;
    }
}
