using UnityEngine;

namespace radio_interaction
{
    //need this state for later
    public class TuningLockState : IRadioState
    {
        private readonly RadioController _radioController;
        public TuningLockState(RadioController radioController) => _radioController = radioController;

        private float timer;

        public void Enter()
        {
            timer = 0;
        }

        public void Update()
        {
            _radioController.PositionTuningCamera();
            _radioController.HandleMouseMovement();
            _radioController.TuneRadio();
            if (!_radioController.OnCorrectChannel())
            {
                _radioController.RadioStateMachine
                    .ChangeState(new TuningState(_radioController));
                return;
            }

            timer += Time.deltaTime;

            if (timer >= _radioController.GetTuningTimer())
            {
                _radioController.RadioStateMachine.ChangeState(new RadioOnState(_radioController));
            }
        }

        public void Exit()
        {
        }
    }
}