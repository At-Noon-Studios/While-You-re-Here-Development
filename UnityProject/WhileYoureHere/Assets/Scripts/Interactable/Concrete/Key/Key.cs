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
        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;

        private IInteractor _interactor;
        private Action<bool> _onSuccess;

        public void StartMinigame(IInteractor interactor, Action<bool> onSuccess)
        {
            _onSuccess = onSuccess;
            _interactor = interactor;
            look.OnRaise += RotateKey;
            cameraController.PauseCameraMovement();
        }

        private void EndMinigame(bool locked)
        {
            look.OnRaise -= RotateKey;
            cameraController.ResumeCameraMovement();
            Interact(_interactor); // make the player pick up the key again
            _onSuccess?.Invoke(locked);
        }

        private void RotateKey(Vector2 mouseDelta)
        {
            transform.Rotate(transform.forward, mouseDelta.x);
            if (DoorLocked) EndMinigame(true);
            if (DoorUnlocked) EndMinigame(false);
        }
        
        private bool DoorLocked => transform.rotation.eulerAngles.z >= 180f;
        private bool DoorUnlocked => transform.rotation.eulerAngles.z <= -180f; // TODO this is some bs because eulerangles is 0 to 360
    }
}