using UnityEngine;

namespace radio_interaction
{
    public class RadioOnState : IRadioState
    {
        private readonly RadioController _radioController;
        public RadioOnState(RadioController radioController) => _radioController = radioController;
        private bool _wasDonePlayingCorrectChannel;

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
            if (_wasDonePlayingCorrectChannel) return;

            if (_radioController.DonePlayingCorrectChannel())
            {
                _wasDonePlayingCorrectChannel = true;
            }
            else _wasDonePlayingCorrectChannel = false;

            if (!_wasDonePlayingCorrectChannel) return;
            _radioController.PlayClassicRadio();
        }
    }
}