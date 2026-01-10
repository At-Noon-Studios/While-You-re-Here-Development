using UnityEngine;

namespace radio_interaction
{
    public class RadioOnState : IRadioState
    {
        private readonly RadioController _radioController;
        public RadioOnState(RadioController radioController) => _radioController = radioController;
        private bool _wasDonePlayingCorrectChannel;
        private float timer;
        public void Enter()
        {
            _radioController.ShowOnCanvas();
        }

        public void Exit()
        {
            _wasDonePlayingCorrectChannel = false;
        }

        public void Update()
        {

            if (_radioController.IsPlayingClassicRadio())
            {
                if (!_radioController.IsAudioSourcePlaying())
                {
                    _radioController.RadioStateMachine.ChangeState(new RadioOffState(_radioController));
                }
            }
            else if (_radioController.DonePlayingCorrectChannel())
            {
                _radioController.PlayClassicRadio();
            }

        }
    }
}