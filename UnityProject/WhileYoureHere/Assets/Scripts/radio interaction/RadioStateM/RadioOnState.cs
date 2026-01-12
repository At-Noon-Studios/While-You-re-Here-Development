namespace radio_interaction
{
    public class RadioOnState : IRadioState
    {
        private readonly RadioController _radioController;

        public RadioOnState(RadioController radioController) =>
            _radioController = radioController;

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (_radioController.IsPlayingClassicRadio())
            {
                if (!_radioController.IsAudioSourcePlaying())
                {
                    _radioController.RadioStateMachine.ChangeState(
                        new RadioOffState(_radioController));
                }
            }
            else if (_radioController.DonePlayingCorrectChannel())
            {
                _radioController.PlayClassicRadio();
            }
        }
    }
}