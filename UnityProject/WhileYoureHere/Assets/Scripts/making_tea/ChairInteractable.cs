using System.Collections;
using Interactable;
using player_controls;
using PlayerControls;
using ScriptableObjects.Gamestate;
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

        [Header("Camera FOV Settings")]
        [SerializeField] [Range(0,180)] private float sitFOV = 60f;
        [SerializeField] private bool changeFOV = true;

        [Header("Start Setting")]
        [SerializeField] private SoGamestateFlag notebookPickedUpFlag;
        [SerializeField] private bool startSitting;
        
        private float _originalFOV;

        private static bool _playerAlreadySeatedAtStart;
        private bool _standUpLocked;
        private bool _isSitting;
        private ChairInteractable _activeChair;

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

        private void Start()
        {
            Debug.Log($"[Chair] Start called on {gameObject.name}, startSitting={startSitting}");
            
            if (!startSitting) return;
            StartCoroutine(StartSittingRoutine());
        }

        private IEnumerator StartSittingRoutine()
        {
            Debug.Log($"[Chair] StartSittingRoutine started on {gameObject.name}");
            
            if (_playerAlreadySeatedAtStart) yield break;
            
            GameObject player = null;
            PlayerInteractionController playerController = null;

            while (player == null || playerController == null)
            {
                player = GameObject.FindWithTag("Player");
                if (player != null)
                    playerController = player.GetComponent<PlayerInteractionController>();
                
                Debug.Log($"[Chair] Waiting for Player... player={player}, pic={playerController}");
                yield return null;
            }
            
            player.SetActive(false);
            
            yield return null;

            if (_playerAlreadySeatedAtStart) yield break;
            
            _playerAlreadySeatedAtStart = true;
            Sit(playerController);
            player.SetActive(true);
            _standUpLocked = true;
        }

        private void Update()
        {
            if (_standUpLocked &&
                notebookPickedUpFlag != null &&
                notebookPickedUpFlag.currentValue)
            {
                Debug.Log("[Chair] StandUp unlocked");
                _standUpLocked = false;
            }
            if (interactionCanvas == null ||
                !interactionCanvas.gameObject.activeSelf ||
                _playerCamera == null) return;
            
            interactionCanvas.transform.LookAt(_playerCamera);
            interactionCanvas.transform.Rotate(0f, 180f, 0f);
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            if (_activeChair != null && _activeChair != this) return;
            
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
            if (_activeChair != null && _activeChair != this) return;
            
            if (!_isSitting)
            {
                Sit(interactor);
                return;
            }

            if (_standUpLocked)
            {
                Debug.Log($"[Chair] StandUp blocked (locked)");
                return;
            }
            
            StandUp();
        }

        private void Sit(IInteractor interactor)
        {
            _activeChair = this;
            
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

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        private void StandUp()
        {
            if (_standUpLocked)
            {
                Debug.Log($"[Chair] StandUp blocked (locked)");
                return;
            }
            
            _activeChair = null;
            
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

            if (_pic == null) return;
            _pic.EnableTableMode(false);
            _pic.ClearSittingChair();
        }

        public void ForceStandUp()
        {
            if (_isSitting && !_standUpLocked)
                StandUp();
        }
    }
}
