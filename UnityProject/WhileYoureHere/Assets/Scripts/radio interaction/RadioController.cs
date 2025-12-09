using player_controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace radio_interaction
{
    public class RadioController : MonoBehaviour
    {
        [Header("audio tracks")]
        [SerializeField] private RadioTracks[] radioTracks;
        [Header("radio data")]
        [HideInInspector] public RadioStateMachine radioStateMachine;
        [SerializeField] private Transform player;
        [SerializeField] private Transform slider;
        [SerializeField] private RadioData radioData;
        [SerializeField] private Transform cam;
        [SerializeField] private Canvas OnCanvas;
        [SerializeField] private Canvas OfTuneCanvas;
        [SerializeField] private Canvas SlideCanvas;
        private AudioSource audioSource;
        private int currentRadioIndex;
        private float tuneValue;
        private Vector2 lastMousePos;
        private MovementController movementController;
        private CameraController cameraController;
        private Vector3 _currentCameraPosition;
        private Quaternion _currentCameraRotation;
        
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            movementController = player.GetComponent<MovementController>();
            cameraController = player.GetComponentInChildren<CameraController>();
            tuneValue = 1f/radioTracks.Length;
            radioStateMachine =  new RadioStateMachine();
            radioStateMachine.ChangeState(new RadioOffState(this));
        }
        
        private void Update()
        {
            radioStateMachine.CurrentState.Update();
        }

        #region public methods
        public float getTuningTimer() => radioData.tuningWaitTime;
        public void PositionTuningCamera()
        {
            var position = Vector3.Lerp(
                cam.transform.position,
                transform.position + -transform.right*2,
                Time.deltaTime * 5f
            );
            cam.position=position;
                
            var targetRotation = Quaternion.LookRotation(
                transform.position - cam.transform.position,
                Vector3.up
            );
                
            cam.transform.rotation = Quaternion.Lerp(
                cam.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }

        public bool ResetCamera()
        {
            const float moveSpeed = 5f;
            const float rotateSpeed = 5f;
            cam.position = Vector3.Lerp(
                cam.position,
                _currentCameraPosition,
                Time.deltaTime * moveSpeed
            );
            cam.rotation = Quaternion.Lerp(
                cam.rotation,
                _currentCameraRotation,
                Time.deltaTime * rotateSpeed
            );

            var positionDone = Vector3.Distance(cam.position, _currentCameraPosition) < 0.01f;
            var rotationDone = Quaternion.Angle(cam.rotation, _currentCameraRotation) < 0.5f;
            return positionDone && rotationDone;
        }

        public void HandleMouseMovement()
        {
            var current = Mouse.current.position.ReadValue();
            var deltaX = current.x - lastMousePos.x;
            tuneValue = Mathf.Clamp01(tuneValue + deltaX * radioData.sensitivity);
            MoveSlider(deltaX);
            lastMousePos = current;
        }
 
        public void EnterTuningMode()
        {
            cameraController?.PauseCameraMovement();
            _currentCameraPosition = cam.transform.position;
            _currentCameraRotation = cam.transform.rotation;
            new WaitForSeconds(2f);

            OnCanvas.gameObject.SetActive(false);
            movementController?.PauseMovement();
            lastMousePos = Mouse.current.position.ReadValue();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        public void ExitTuningMode()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            movementController?.ResumeMovement();
            cameraController?.ResumeCameraMovement();
            OnCanvas.gameObject.SetActive(true);
        }
        public void OnPowerPressed()
        {
            switch  (radioStateMachine.CurrentState)
            {
               case RadioOffState:
                   radioStateMachine.ChangeState(new RadioOnState(this));
                   break;
               case RadioOnState:
               case TuningState:
                   radioStateMachine.ChangeState(new RadioOffState(this));
                   break;
            }
        }
        public void OnTunePressed()
        {
            switch (radioStateMachine.CurrentState)
            {
                case RadioOnState:
                    radioStateMachine.ChangeState(new TuningState(this));
                    break;
                case TuningState:
                    radioStateMachine.ChangeState(new RadioOnState(this));
                    break;
            } 
        }
        public void SlideCanvasStatus(bool value) {SlideCanvas.gameObject.SetActive(value);} 
        public void ShowOffCanvas()
        {
            OfTuneCanvas.gameObject.SetActive(true);
            OnCanvas.gameObject.SetActive(false);
            SlideCanvas.gameObject.SetActive(false);
        }

        public void ShowOnCanvas()
        {
            OnCanvas.gameObject.SetActive(true);
            OfTuneCanvas.gameObject.SetActive(false);
            SlideCanvas.gameObject.SetActive(false);
        }
        public void TurnRadioOn()
        {
            if (currentRadioIndex == 0)
            {
                PlayStatic();
            }
            else PlayClip(radioTracks[currentRadioIndex].audioClip);
        }


        public void TurnRadioOff()
        {
            audioSource.Stop();
        }

        public void TuneRadio()
        {
            var stationSpacing = 1f / radioTracks.Length;
            var newIndex = Mathf.FloorToInt(tuneValue / stationSpacing);
            newIndex = Mathf.Clamp(newIndex, 0, radioTracks.Length - 1);
            if (currentRadioIndex == newIndex && audioSource.clip == radioTracks[newIndex].audioClip) return;
            currentRadioIndex = newIndex;
            PlayClip(radioTracks[newIndex].audioClip);
        }
        public bool OnCorrectChannel() => radioTracks[currentRadioIndex].audioClip == radioTracks[2].audioClip;
        #endregion
        #region private methods
        private void PlayClip(AudioClip clip)
        {
            if (audioSource.clip == clip && audioSource.isPlaying) return;
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        private void MoveSlider(float deltaX)
        {
            var move = deltaX *radioData.SliderSensitivity();
            var pos = slider.localPosition;
            pos.z -= move;
            pos.z = Mathf.Clamp(pos.z, radioData.MinSliderPos(), radioData.MaxSliderPos());
            slider.localPosition = pos;
        }
        private void PlayStatic ()=> PlayClip(radioTracks[0].audioClip);
        #endregion
       
    }
}