using Interactable;
using player_controls;
using PlayerControls;
using TaskList;
using UnityEngine;

namespace making_tea
{
    public class ChairInteractable : InteractableBehaviour
    {
        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        [Header("References")]
        [SerializeField] private Transform sitPoint;
        [SerializeField] private Transform lookTarget;

        [Header("Camera Sitting Position Offset")]
        [SerializeField] private Vector3 cameraSitOffset = Vector3.zero;

        [Header("Camera Sitting Rotation Offset")]
        [SerializeField] private Vector3 cameraSitRotationOffset = Vector3.zero;

        [Header("Camera FOV Settings")]
        [SerializeField, Range(0, 180)] private float sitFOV = 60f;
        [SerializeField] private bool changeFOV = true;

        private float _originalFOV;
        private bool _isSitting;

        private PlayerInteractionController _pic;
        private MovementController _movement;
        private CameraController _cameraController;
        private Transform _player;
        private Camera _playerCam;

        private Transform _playerCamera;

        private Vector3 _originalCameraLocalPos;
        private Quaternion _originalCameraLocalRot;

        private TaskListHintController _taskListHintController;

        protected override void Awake()
        {
            base.Awake();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerCamera = player.GetComponentInChildren<Camera>()?.transform;

            _taskListHintController = Object.FindFirstObjectByType<TaskListHintController>();
        }

        private void Update()
        {
            if (interactionCanvas == null ||
                !interactionCanvas.gameObject.activeSelf ||
                _playerCamera == null) return;

            interactionCanvas.transform.LookAt(_playerCamera);
            interactionCanvas.transform.Rotate(0f, 180f, 0f);
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            if (!_isSitting && interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(true);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor) => string.Empty;

        public override void Interact(IInteractor interactor)
        {
            if (_isSitting) return;
            Sit(interactor);
        }

        private void Sit(IInteractor interactor)
        {
            if (interactor is not PlayerInteractionController p)
                return;

            _player = p.transform;
            _movement = p.GetComponent<MovementController>();
            _playerCam = p.GetComponentInChildren<Camera>();
            _cameraController = p.GetComponentInChildren<CameraController>();

            if (_movement != null) _movement.enabled = false;
            if (_cameraController != null) _cameraController.enabled = false;

            _player.position = sitPoint.position;
            _player.rotation = sitPoint.rotation;

            if (_playerCam != null)
            {
                _originalCameraLocalPos = _playerCam.transform.localPosition;
                _originalCameraLocalRot = _playerCam.transform.localRotation;
                _originalFOV = _playerCam.fieldOfView;
            }

            if (lookTarget != null && _playerCam != null)
            {
                var dir = lookTarget.position - _playerCam.transform.position;
                _playerCam.transform.rotation = Quaternion.LookRotation(dir);
            }

            if (_playerCam != null)
                _playerCam.transform.localPosition += cameraSitOffset;

            if (_playerCam != null)
                _playerCam.transform.localRotation *= Quaternion.Euler(cameraSitRotationOffset);

            if (changeFOV && _playerCam != null)
                _playerCam.fieldOfView = sitFOV;

            _isSitting = true;

            _pic = p;
            _pic.EnableTableMode(true);
            _pic.SetSittingChair(this);

            _taskListHintController?.SetHintsHidden(true);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public void StandUp()
        {
            if (!_isSitting) return;

            if (_movement != null) _movement.enabled = true;
            if (_cameraController != null) _cameraController.enabled = true;

            if (_playerCam != null)
            {
                _playerCam.transform.localPosition = _originalCameraLocalPos;
                _playerCam.transform.localRotation = _originalCameraLocalRot;

                if (changeFOV)
                    _playerCam.fieldOfView = _originalFOV;
            }

            _isSitting = false;

            if (_pic != null)
            {
                _pic.EnableTableMode(false);
                _pic.ClearSittingChair();
            }

            _taskListHintController?.SetHintsHidden(false);
        }
    }
}
