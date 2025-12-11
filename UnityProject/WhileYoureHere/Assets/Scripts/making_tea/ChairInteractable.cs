using Interactable;
using player_controls;
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
        [SerializeField] private Vector3 cameraSitOffset = new Vector3(0f, 0f, 0f);

        [Header("Camera Sitting Rotation Offset")]
        [SerializeField] private Vector3 cameraSitRotationOffset = new Vector3(0f, 0f, 0f);

        private bool _isSitting;

        private PlayerInteractionController _pic;
        private MovementController _movement;
        private CameraController _cameraController;
        private Transform _player;
        private Camera _playerCam;

        private Transform _playerCamera;

        private Vector3 _originalCameraLocalPos;
        private Quaternion _originalCameraLocalRot;

        protected override void Awake()
        {
            base.Awake();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerCamera = player.GetComponentInChildren<Camera>()?.transform;
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

        public override void Interact(IInteractor interactor)
        {
            if (!_isSitting)
                Sit(interactor);
            else
                StandUp();
        }

        private void Sit(IInteractor interactor)
        {
            if (interactor is not PlayerInteractionController p)
            {
                Debug.LogWarning("ChairInteractable: Interactor is not a PlayerInteractionController!");
                return;
            }

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

            _isSitting = true;

            _pic = p;
            _pic.EnableTableMode(true);
            _pic.SetSittingChair(this);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        private void StandUp()
        {
            if (_movement != null) _movement.enabled = true;
            if (_cameraController != null) _cameraController.enabled = true;

            if (_playerCam != null)
            {
                _playerCam.transform.localPosition = _originalCameraLocalPos;
                _playerCam.transform.localRotation = _originalCameraLocalRot;
            }

            _isSitting = false;

            if (_pic == null) return;
            _pic.EnableTableMode(false);
            _pic.ClearSittingChair();
        }

        public void ForceStandUp()
        {
            if (_isSitting)
                StandUp();
        }
    }
}
