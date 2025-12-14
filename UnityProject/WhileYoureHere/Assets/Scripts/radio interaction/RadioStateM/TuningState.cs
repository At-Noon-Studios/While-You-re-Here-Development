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
            Debug.Log("Entered Tuning State");
            _radioController.EnterTuningMode();
            _radioController.SlideCanvasStatus(true);
            timer = 0;
        }

        public void Exit()
        {
            Debug.Log("Exited Tuning State");
            _radioController.ExitTuningMode();
        }

        public void Update()
        {
            Debug.Log("Updating Tuning State");
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
            Debug.Log("timer is "+ timer);
            if (timer >= _radioController.GetTuningTimer())
            {
                Debug.Log("Tuning done");
                _radioController.RadioStateMachine.ChangeState(new ResetCameraState(_radioController));
            }
        }
    }
}