using EventChannels;
using JetBrains.Annotations;
using UnityEngine;

namespace PlayerControls
{
    public class PlayerInteractionController : MonoBehaviour
    {
        [Header("Listen to")]
        [SerializeField] private Vector2EventChannel look;

        [SerializeField] private Material outlineMaterial;
        [SerializeField] private Camera playerCamera;
        [CanBeNull] private Collider _currentHit;
        [CanBeNull] private IInteractable _currentHitInteractable;

        private void OnEnable()
        {
            look.OnRaise += OnLook;
            
        }

        private void OnDisable()
        {
            look.OnRaise -= OnLook;
        }

        private void OnLook(Vector2 _)
        {
            var ray = playerCamera.ScreenPointToRay(Vector3.zero);
            if (!Physics.Raycast(ray, out var hit)) return;
            var hitCollider = hit.collider;
            if (_currentHit == hitCollider) return;
            // _currentHit.GetComponent<Renderer>().materials REMOVE OUTLINE MATERIAL FROM ARRAY
            _currentHit = hitCollider;
            // _currentHit.GetComponent<Renderer>().materials ADD OUTLINE MATERIAL TO ARRAY
            _currentHitInteractable = hitCollider.GetComponent<IInteractable>();
        }

        //Action input callback
        private void OnInteract()
        {
            _currentHitInteractable?.Interact();
        }
    }
}
