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

        public void Enter()
        {
            Debug.Log("Entered Tuning State");
            _radioController.EnterTuningMode();
            _radioController.ShowSlideCanvas(true);
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
            _radioController.PositionTuningCamera();
            _radioController.HandleMouseMovement();
            _radioController.TuneRadio();



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