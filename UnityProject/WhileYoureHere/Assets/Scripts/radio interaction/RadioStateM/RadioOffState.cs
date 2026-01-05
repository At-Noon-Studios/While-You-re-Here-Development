namespace radio_interaction
{
    public class RadioOffState : IRadioState
    {
        private readonly RadioController _radioController;
        public RadioOffState(RadioController radioController) => _radioController = radioController;

        public void Enter()
        {
            _radioController.ShowOffCanvas();
            _radioController.TurnRadioOff();
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}