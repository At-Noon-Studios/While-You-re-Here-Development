using EventChannels;
using Interactable;
using JetBrains.Annotations;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    public class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private PlayerInteractionData data;
        [SerializeField] private Camera playerCamera;
        
        [CanBeNull] private IInteractable _currentTarget;

        private void Update()
        {
            RefreshCurrentTarget();
        }

        private void RefreshCurrentTarget()
        {
            var ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            Debug.DrawRay(ray.origin, ray.direction.normalized * data.InteractionReach, Color.red, 0.01f);
            
            if (!Physics.Raycast(ray, out var hit, data.InteractionReach))
            {
                _currentTarget?.OnHoverExit();
                _currentTarget = null;
                return;
            }
            
            hit.collider.TryGetComponent(out IInteractable newTarget);
            if (newTarget == _currentTarget) return;
            _currentTarget?.OnHoverExit();
            _currentTarget = newTarget;
            newTarget?.OnHoverEnter();
        }
        
        //Action input callback
        private void OnInteract()
        {
            _currentTarget?.Interact();
        }
    }
}
