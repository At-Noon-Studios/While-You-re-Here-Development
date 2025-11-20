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
        [Header("Listen to")]
        [SerializeField] private EventChannel interact;
        
        [CanBeNull] private IInteractable _currentTarget;

        private void Update()
        {
            RefreshCurrentTarget();
        }

        private void OnEnable()
        {
            interact.OnRaise += Interact;
        }

        private void OnDisable()
        {
            interact.OnRaise -= Interact;
        }

        private void RefreshCurrentTarget()
        {
            var ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, data.InteractionReach))
            {
                SetCurrentTarget(null);
                return;
            }
            hit.collider.TryGetComponent(out IInteractable newTarget);
            if (newTarget == _currentTarget) return;
            SetCurrentTarget(newTarget);
        }

        private void SetCurrentTarget(IInteractable newTarget)
        {
            _currentTarget?.OnHoverExit();
            _currentTarget = newTarget;
            newTarget?.OnHoverEnter();
        }
        
        private void Interact()
        {
            _currentTarget?.Interact();
        }
    }
}
