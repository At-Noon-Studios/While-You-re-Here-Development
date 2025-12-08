namespace radio_interaction
{
    public class RadioStateMachine
    {
        public IRadioState CurrentState { get; private set; }

        public void ChangeState(IRadioState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
        public void Update() => CurrentState.Update();
    }
}