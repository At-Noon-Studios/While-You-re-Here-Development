using player_controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace radio_interaction
{
    public class RadioController : MonoBehaviour
    {
        [Header("audio tracks")] [SerializeField]
        private RadioTracks[] radioTracks;

        [Header("radio data")] [HideInInspector]
        public RadioStateMachine RadioStateMachine;

        [SerializeField] private Transform player;
        [SerializeField] private Transform slider;
        [SerializeField] private RadioData radioData;
        [SerializeField] private Transform cam;
        [SerializeField] private Canvas onStateCanvas;
        [SerializeField] private Canvas offStateCanvas;
        [SerializeField] private Canvas slideCanvas;
        private AudioSource _audioSource;
        private int _currentRadioIndex;
        private float _tuneValue;
        private Vector2 _lastMousePos;
        private MovementController _movementController;
        private CameraController _cameraController;
        private Vector3 _currentCameraPosition;
        private Quaternion _currentCameraRotation;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _movementController = player.GetComponent<MovementController>();
            _cameraController = player.GetComponentInChildren<CameraController>();
            _tuneValue = 1f / radioTracks.Length;
            RadioStateMachine = new RadioStateMachine();
            RadioStateMachine.ChangeState(new RadioOffState(this));
        }

        private void Update()
        {
            RadioStateMachine.Update();
        }

        #region public methods

        public float GetTuningTimer() => radioData.tuningWaitTime;

        public void PositionTuningCamera()
        {
            var position = Vector3.Lerp(
                cam.transform.position,
                transform.position + -transform.right * 2,
                Time.deltaTime * 5f
            );
            cam.position = position;

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
            var deltaX = current.x - _lastMousePos.x;
            _tuneValue = Mathf.Clamp01(_tuneValue + deltaX * radioData.sensitivity);
            MoveSlider(deltaX);
            _lastMousePos = current;
        }

        public void EnterTuningMode()
        {
            _cameraController?.PauseCameraMovement();
            _currentCameraPosition = cam.transform.position;
            _currentCameraRotation = cam.transform.rotation;
            onStateCanvas.gameObject.SetActive(false);
            _movementController?.PauseMovement();
            _lastMousePos = Mouse.current.position.ReadValue();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void ExitTuningMode()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _movementController?.ResumeMovement();
            _cameraController?.ResumeCameraMovement();
            onStateCanvas.gameObject.SetActive(true);
        }

        public void OnPowerPressed()
        {
            switch (RadioStateMachine.CurrentState)
            {
                case RadioOffState:
                    RadioStateMachine.ChangeState(new RadioOnState(this));
                    break;
                case RadioOnState:
                case TuningState:
                    RadioStateMachine.ChangeState(new RadioOffState(this));
                    break;
            }
        }

        public void OnTunePressed()
        {
            switch (RadioStateMachine.CurrentState)
            {
                case RadioOnState:
                    RadioStateMachine.ChangeState(new TuningState(this));
                    break;
                case TuningState:
                    RadioStateMachine.ChangeState(new RadioOnState(this));
                    break;
            }
        }

        public void SlideCanvasStatus(bool value)
        {
            slideCanvas.gameObject.SetActive(value);
        }

        public void ShowOffCanvas()
        {
            offStateCanvas.gameObject.SetActive(true);
            onStateCanvas.gameObject.SetActive(false);
            slideCanvas.gameObject.SetActive(false);
        }

        public void ShowOnCanvas()
        {
            onStateCanvas.gameObject.SetActive(true);
            offStateCanvas.gameObject.SetActive(false);
            slideCanvas.gameObject.SetActive(false);
        }

        public void TurnRadioOn()
        {
            if (_currentRadioIndex == 0)
            {
                PlayStatic();
            }
            else PlayClip(radioTracks[_currentRadioIndex].audioClip);
        }


        public void TurnRadioOff()
        {
            _audioSource.Stop();
        }

        public void TuneRadio()
        {
            var stationSpacing = 1f / radioTracks.Length;
            var newIndex = Mathf.FloorToInt(_tuneValue / stationSpacing);
            newIndex = Mathf.Clamp(newIndex, 0, radioTracks.Length - 1);
            if (_currentRadioIndex == newIndex && _audioSource.clip == radioTracks[newIndex].audioClip) return;
            _currentRadioIndex = newIndex;
            PlayClip(radioTracks[newIndex].audioClip);
        }

        public bool OnCorrectChannel() => radioTracks[_currentRadioIndex].audioClip == radioTracks[2].audioClip;

        #endregion

        #region private methods

        private void PlayClip(AudioClip clip)
        {
            if (_audioSource.clip == clip && _audioSource.isPlaying) return;
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        private void MoveSlider(float deltaX)
        {
            var move = deltaX * radioData.SliderSensitivity();
            var pos = slider.localPosition;
            pos.z -= move;
            pos.z = Mathf.Clamp(pos.z, radioData.MinSliderPos(), radioData.MaxSliderPos());
            slider.localPosition = pos;
        }

        private void PlayStatic() => PlayClip(radioTracks[0].audioClip);

        #endregion
    }
}