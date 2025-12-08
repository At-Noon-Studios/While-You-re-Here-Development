using UnityEngine;

namespace radio_interaction
{
    public class TuningLockState: IRadioState
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
            _radioController. PositionCamera();
            _radioController.HandleMouseMovement();
            _radioController.TuneRadio();
            if (!_radioController.OnCorrectChannel())
            {
                _radioController.radioStateMachine
                    .ChangeState(new TuningState(_radioController));
                return;
            }

            timer +=Time.deltaTime;
            ;
            if (timer >= _radioController.getTuningTimer())
            {
                _radioController.radioStateMachine.ChangeState(new RadioOnState(_radioController));                
            }
        }
        
        public void Exit()
        {
            _radioController.ExitTuningMode();
        }
    }
}