using Unity.VisualScripting;
using UnityEngine;

namespace radio_interaction
{
    public class TuningState:IRadioState
        
    {
        private readonly RadioController _radioController;
        public TuningState(RadioController radioController) => _radioController = radioController;
        private float timer;
        private const float sliderLifeTime = 5f;

        public void Enter()
        {
            _radioController.EnterTuningMode();
            timer = 0;
        }

        public void Exit()
        {
            _radioController.ExitTuningMode();
        }

        public void Update()
        {
            _radioController.SlideCanvasStatus(true);
            timer+= Time.deltaTime;
            _radioController. PositionCamera();
            _radioController.HandleMouseMovement();
            _radioController.TuneRadio();

            if (timer >= sliderLifeTime)
            {
                _radioController.SlideCanvasStatus(false);
            }
            if (_radioController.OnCorrectChannel())
            {
                if (timer >= _radioController.getTuningTimer())
                {
                    _radioController.radioStateMachine.ChangeState(new TuningLockState(_radioController));
                }
            }
        }
    }
}