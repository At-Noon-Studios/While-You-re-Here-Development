using UnityEngine;

namespace radio_interaction
{
    public class RadioOnState : IRadioState
    {
        private readonly RadioController _radioController;
        public RadioOnState(RadioController radioController) => _radioController = radioController;
        private float timer;
        public void Enter()
        {
            _radioController.ShowOnCanvas();
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (_radioController.DonePlayingCorrectChannel())
            {
                timer += Time.deltaTime;
            }
            
            if (timer >= _radioController.GetTuningTimer())
            {
                _radioController.PlayClassicRadio();
            }
        }
    }
}