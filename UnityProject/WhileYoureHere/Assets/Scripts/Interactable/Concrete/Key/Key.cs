using System;
using EventChannels;
using Interactable.Holdable;
using player_controls;
using UnityEngine;

namespace Interactable.Concrete.Key
{
    public class Key : HoldableObjectBehaviour
    {
        [Header("Camera controller")]
        [SerializeField] private CameraController cameraController;
        [Header("Movement controller")]
        [SerializeField] private MovementController movementController;
        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;

        private IInteractor _interactor;
        private Action<bool> _onSuccess;

        private float _rotation;
        
        public void StartMinigame(IInteractor interactor, Action<bool> onSuccess)
        {
            _onSuccess = onSuccess;
            _interactor = interactor;
            look.OnRaise += RotateKey;
            cameraController.PauseCameraMovement();
            movementController.PauseMovement();
        }

        private void EndMinigame(bool locked)
        {
            look.OnRaise -= RotateKey;
            cameraController.ResumeCameraMovement();
            movementController.ResumeMovement();
            Interact(_interactor); // make the player pick up the key again
            _onSuccess?.Invoke(locked);
            _rotation = 0;
        }

        private void RotateKey(Vector2 mouseDelta)
        {
            _rotation += mouseDelta.x;
            var currentRotation = transform.rotation;
            transform.rotation =  Quaternion.Euler(currentRotation.x, currentRotation.y, _rotation);
            if (DoorLocked) EndMinigame(true);
            if (DoorUnlocked) EndMinigame(false);
        }
        
        private bool DoorLocked => _rotation >= -180f;
        private bool DoorUnlocked => _rotation <= 180f;
    }
}