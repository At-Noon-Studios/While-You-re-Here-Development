namespace radio_interaction
{
    public class RadioOnState : IRadioState
    {
        private readonly RadioController _radioController;
        public RadioOnState(RadioController radioController) => _radioController = radioController;

        public void Enter()
        {
            _radioController.ShowOnCanvas();
        }

        public void Exit()
        {
        }

        public void Update()
        {

        }
    }
}