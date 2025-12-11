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
        private const float sliderLifeTime = 5f;

        public void Enter()
        {
            _radioController.EnterTuningMode();
            _radioController.SlideCanvasStatus(true);
            timer = 0;
        }

        public void Exit()
        {
            _radioController.ExitTuningMode();
        }

        public void Update()
        {
            sliderTimer += Time.deltaTime;
            _radioController.PositionTuningCamera();
            _radioController.HandleMouseMovement();
            _radioController.TuneRadio();

            if (sliderTimer >= sliderLifeTime)
            {
                _radioController.SlideCanvasStatus(false);
            }

            if (!_radioController.OnCorrectChannel())
            {
                return;
            }

            if (_radioController.OnCorrectChannel())
            {
                timer += Time.deltaTime;
            }
            else timer = 0;

            if (timer >= _radioController.GetTuningTimer())
            {
                _radioController.RadioStateMachine.ChangeState(new ResetCameraState(_radioController));
            }
        }
    }
}