using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace radio_interaction
{
    public class TuningState : IRadioState
    {
        private readonly RadioController _radioController;
        public TuningState(RadioController radioController) => _radioController = radioController;
        private float timer;
        private float sliderTimer;
        private const float SliderLifeTime = 5f;

        public void Enter()
        {
            _radioController.EnterTuningMode();
            _radioController.ShowSlideCanvas(true);
            timer = 0;
        }

        public void Exit()
        {
            _radioController.ExitTuningMode();
        }

        public void Update()
        {
            _radioController.PositionTuningCamera();
            _radioController.HandleMouseMovement();
            _radioController.TuneRadio();


            if (_radioController.OnCorrectChannel())
            {
                timer += Time.deltaTime;
                if (timer >= _radioController.GetTuningTimer())
                {
                    _radioController.RadioStateMachine.ChangeState(
                        new ResetCameraState(_radioController));
                }
            }
            else
            {
                timer = 0;
            }
        }
    }
}