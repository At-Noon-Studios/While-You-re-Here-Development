using EventChannels;
using Interactable;
using Interactable.Concrete.Key;
using JetBrains.Annotations;
using player_controls;
using UnityEngine;

namespace door
{
    public class Keyhole : InteractableBehaviour
    {
        [Header("Key Placement Settings")]
        [SerializeField] private Transform keyHolePosition;
        [SerializeField] private Vector3 keyRotation;
        
        [Header("Camera controller")]
        [SerializeField] private CameraController cameraController;
        [Header("Movement controller")]
        [SerializeField] private MovementController movementController;
        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;

        [CanBeNull] private IInteractor _currentOperator;
        [CanBeNull] private Key _currentKey;
        
        public bool IsLocked { get; private set; }
        
        public override void Interact(IInteractor interactor)
        {
            if (CurrentlyBeingOperated && CanStopOperating(out var lockedState))
            {
                FinishOperatingLock(lockedState);
                return;
            }
            if (interactor.HeldObject is not Key key) return;
            StartOperatingLock(interactor, key);
        }

        private void StartOperatingLock(IInteractor interactor, Key key)
        {
            _currentOperator = interactor;
            _currentKey = key;
            key.Place(keyHolePosition.position, Quaternion.Euler(keyRotation));
            key.detectable = false;
            look.OnRaise += RotateKey;
            cameraController.PauseCameraMovement();
            movementController.PauseMovement();
        }

        private void FinishOperatingLock(bool isLocked)
        {
            if (!CurrentlyBeingOperated) Debug.LogError("This is not supposed to happen");
            StopOperatingLock();
            IsLocked = isLocked;
        }

        private void StopOperatingLock()
        {
            if (!CurrentlyBeingOperated) Debug.LogError("This is not supposed to happen");
            _currentKey?.Interact(_currentOperator);
            _currentKey?.ResetRotation();
            _currentKey!.detectable = true;
            _currentOperator = null;
            _currentKey = null;
            look.OnRaise -= RotateKey;
            cameraController.ResumeCameraMovement();
            movementController.ResumeMovement();
        }

        private void RotateKey(Vector2 mouseDelta)
        {
            if (!CurrentlyBeingOperated) Debug.LogError("This is not supposed to happen");
            _currentKey?.RotateKey(mouseDelta.x);
        }

        public bool CurrentlyBeingOperated => _currentOperator != null && _currentKey != null;

        public override bool IsDetectableBy(IInteractor interactor)
        {
            return (interactor.HeldObject is Key && !CurrentlyBeingOperated) || (CurrentlyBeingOperated && CanStopOperating(out _));
        }

        public override string InteractionText(IInteractor interactor)
        {
            if (CurrentlyBeingOperated && CanStopOperating(out var lockedState)) return (lockedState ? "Lock" : "Unlock") + " the door";
            return "Operate the lock";
        }

        private bool CanStopOperating(out bool lockedState)
        {
            lockedState = IsLocked;
            switch (_currentKey?.Rotation)
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
    }
}