namespace radio_interaction
{
    public class ResetCameraState : IRadioState
    {
        private readonly RadioController _radioController;
        public ResetCameraState(RadioController radioController) => _radioController = radioController;

        public void Enter()
        {
            _radioController.EnterResetCam();
        }

        public void Exit()
        {
            _radioController.ExitResetCam();
        }

        public void Update()
        {
            if (_radioController.ResetCamera())
            {
                _radioController.RadioStateMachine.ChangeState(new RadioOnState(_radioController));
            }
        }
    }
}