using UnityEngine;

namespace radio_interaction
{
    public class TuningState : IRadioState
    {
        private readonly RadioController _radioController;

        public TuningState(RadioController radioController) =>
            _radioController = radioController;

        private float timer;

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