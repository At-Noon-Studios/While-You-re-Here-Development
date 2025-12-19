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
        }

        public void Update()
        {
            if (_wasDonePlayingCorrectChannel)
                return;

            if (_radioController.DonePlayingCorrectChannel())
            {
                Debug.Log("Playing Classic Radio");
                _radioController.PlayClassicRadio();
                _wasDonePlayingCorrectChannel = true;
            }


        }
    }
}