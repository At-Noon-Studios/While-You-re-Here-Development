using System.Collections;
using System.Linq;
using Interactable;
using Interactable.Concrete.Key;
using player_controls;
using PlayerControls;
using ScriptableObjects.Events;
using UI;
using UnityEngine;

namespace door
{
    public class Keyhole : InteractableBehaviour
    {
        [Header("Key Placement Settings")]
        [SerializeField] private Transform keyHolePosition;
        [SerializeField] private Vector3 keyRotation;

        private CameraController _cameraController;
        private MovementController _movementController;

        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;
        [SerializeField] private EventChannel interact;

        private UIManager _uiManager;
        private DoorInteractable _door;

        private Operation _currentOperation;

        public enum UnlockDirection
        {
            TurnRightIsUnlock,
            TurnLeftIsUnlock
        }

        [Header("Lock Logic")]
        [SerializeField] private UnlockDirection unlockDirection = UnlockDirection.TurnRightIsUnlock;

        protected override void Awake()
        {
            base.Awake();
            FindDoorInParentOrParentSibling();
            if (_door == null) Debug.LogError("Keyhole should have a " + nameof(DoorInteractable) + " parent", this);
        }

        private void Start()
        {
            _uiManager = UIManager.Instance;
            if (_uiManager == null) Debug.LogError("UI Manager not found");

            var player = GameObject.FindWithTag("Player");

            _cameraController = player.GetComponentInChildren<CameraController>();
            if (_cameraController == null) Debug.LogError("Camera controller not found");
            _movementController = player.GetComponentInChildren<MovementController>();
            if (_movementController == null) Debug.LogError("Movement controller not found");
        }

        private void OnEnable()
        {
            interact.OnRaise += AttemptFinishOperatingLock;
        }

        private void OnDisable()
        {
            interact.OnRaise -= AttemptFinishOperatingLock;
        }

        public override void Interact(IInteractor interactor)
        {
            if (interactor.HeldObject is not Key key) return;
            StartOperatingLock(new Operation(key, interactor));
        }

        public override bool IsDetectableBy(IInteractor interactor)
        {
            return base.IsDetectableBy(interactor) && CanStartOperating(interactor) && !_door.isOpen && _door.IsFinishedMoving();
        }

        public override string InteractionText(IInteractor interactor) => "Operate the lock";

        private void AttemptFinishOperatingLock()
        {
            StartCoroutine(DelayInteract());
        }

        private void StartOperatingLock(Operation operation)
        {
            _currentOperation = operation;
            PlaceKey();
            look.OnRaise += RotateKey;
            PausePlayer();
        }

        private void PlaceKey()
        {
            if (!CurrentlyBeingOperated) return;

            Quaternion baseRotation = keyHolePosition.rotation * Quaternion.Euler(keyRotation) * Quaternion.AngleAxis(180f, Vector3.up);

            _currentOperation.Key.Place(keyHolePosition.position, baseRotation);
            _currentOperation.Key.transform.SetParent(keyHolePosition, true);
            _currentOperation.Key.SetBaseRotation(baseRotation);
            _currentOperation.Key.detectable = false;
        }

        private void PausePlayer()
        {
            _cameraController.PauseCameraMovement();
            _movementController.PauseMovement();
        }

        private void FinishOperatingLock(bool isLocked)
        {
            StopOperatingLock();
            _door.isLocked = isLocked;
        }

        private void StopOperatingLock()
        {
            if (!CurrentlyBeingOperated) return;
            ResetCurrentKey();
            _currentOperation = null;
            look.OnRaise -= RotateKey;
            ResumePlayer();
        }

        public bool CurrentlyBeingOperated => _currentOperation != null;

        private void ResetCurrentKey()
        {
            if (!CurrentlyBeingOperated) return;
            _currentOperation.Key.Interact(_currentOperation.Interactor);
            _currentOperation.Key.ResetRotation();
            _currentOperation.Key.ResetPose();
            _currentOperation.Key.detectable = true;
        }

        private IEnumerator DelayInteract()
        {
            yield return new WaitForEndOfFrame();
            if (!CanStopOperating(out var lockedState)) yield break;
            FinishOperatingLock(lockedState);
        }

        private void ResumePlayer()
        {
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
        }

        private void RotateKey(Vector2 mouseDelta)
        {
            _currentOperation.Key.RotateKey(-mouseDelta.x);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (CanStopOperating(out var lockedState))
            {
                _uiManager.ShowInteractPrompt((lockedState ? "Lock" : "Unlock") + " the door", true);
                return;
            }
            _uiManager.HideInteractPrompt();
        }

        private bool CanStartOperating(IInteractor interactor)
        {
            return interactor.HeldObject is Key key && key.Keyholes.Contains(this) && _currentOperation == null;
        }

        private bool CanStopOperating(out bool lockedState)
        {
            lockedState = _door.isLocked;
            if (!CurrentlyBeingOperated) return false;

            float rot = _currentOperation.Key.Rotation;

            if (unlockDirection == UnlockDirection.TurnRightIsUnlock)
            {
                if (rot <= -180f)
                {
                    lockedState = false;
                    return true;
                }

                if (rot >= 180f)
                {
                    lockedState = true;
                    return true;
                }
            }
            else
            {
                if (rot >= 180f)
                {
                    lockedState = false;
                    return true;
                }

                if (rot <= -180f)
                {
                    lockedState = true;
                    return true;
                }
            }

            return false;
        }

        private void FindDoorInParentOrParentSibling()
        {
            _door = GetComponentInParent<DoorInteractable>();
            if (_door != null) return;
            var parent = transform.parent;
            if (_door != null || transform.parent == null) return;
            _door = parent.GetComponentInChildren<DoorInteractable>();
        }

        private class Operation
        {
            public readonly Key Key;
            public readonly IInteractor Interactor;

            public Operation(Key key, IInteractor interactor)
            {
                Key = key;
                Interactor = interactor;
            }
        }
    }
}