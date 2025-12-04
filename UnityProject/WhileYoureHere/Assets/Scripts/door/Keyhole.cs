using Interactable;
using Interactable.Concrete.Key;
using player_controls;
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
        
        [HideInInspector] public bool detectable = true;
        public bool IsLocked { get; private set; }
        public void LockDoor(bool locked) { IsLocked = locked; }
        
        private Operation _currentOperation;

        #region Unity event functions
        
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
        
        #endregion
        
        #region Overrides 

        public override void Interact(IInteractor interactor)
        {
            if (interactor.HeldObject is not Key key) return;
            StartOperatingLock(new Operation(key, interactor));
        }
        
        public override bool IsDetectableBy(IInteractor interactor)
        {
            return CanStartOperating(interactor) && detectable;
        }
        
        public override string InteractionText(IInteractor interactor) => "Operate the lock";
        
        #endregion
        
        #region Private methods
        
        private void AttemptFinishOperatingLock()
        {
            if (!CanStopOperating(out var lockedState)) return;
            FinishOperatingLock(lockedState);
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
            _currentOperation.Key.Place(keyHolePosition.position, Quaternion.Euler(keyRotation));
            _currentOperation.Key.transform.SetParent(transform);
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
            IsLocked = isLocked;
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
            _currentOperation.Key.detectable = true;
        }

        private void ResumePlayer()
        {
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
        }

        private void RotateKey(Vector2 mouseDelta)
        {
            _currentOperation.Key.RotateKey(mouseDelta.x);
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

        private bool CanStartOperating(IInteractor interactor) => interactor.HeldObject is Key && _currentOperation == null;

        private bool CanStopOperating(out bool lockedState)
        {
            lockedState = IsLocked;
            if (!CurrentlyBeingOperated) return false;
            switch (_currentOperation.Key.Rotation)
            {
                case >= 180f:
                    lockedState = true;
                    return true;
                case <= -180f:
                    lockedState = false;
                    return true;
            }
            return false;
        }
        
        #endregion

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