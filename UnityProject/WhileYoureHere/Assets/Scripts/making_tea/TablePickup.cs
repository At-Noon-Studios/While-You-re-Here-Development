using Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public abstract class TablePickup : InteractableBehaviour, ITablePickup
    {
        [Header("Table Settings")]
        [SerializeField] private LayerMask tableLayer;
        [SerializeField] private float liftHeight = 0.15f;

        protected bool IsLifted;
        protected PlayerInteractionController Pic;

        private Rigidbody _rb;

        private const float FixedRayDistance = 10f;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            if (!IsLifted || Pic == null || !Pic.IsTableMode)
                return;

            FollowMouse();
        }

        private void FollowMouse()
        {
            var cam = Pic.PlayerCamera;
            var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, FixedRayDistance, tableLayer))
                return;

            transform.position = hit.point + Vector3.up * liftHeight;
        }

        public bool IsTableHeld => IsLifted;

        public virtual void Pickup(PlayerInteractionController p)
        {
            IsLifted = true;
            Pic = p;

            if (_rb != null)
            {
                _rb.isKinematic = true;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            EnableCollider(false);
        }

        public virtual void Drop()
        {
            IsLifted = false;
            Pic = null;

            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.isKinematic = false;
            }

            EnableCollider(true);
        }

        public virtual void ForceDropFromTableMode()
        {
            Drop();
        }

        public override string InteractionText(IInteractor interactor) => string.Empty;

        public bool InteractableBy(IInteractor interactor)
        {
            return (interactor as PlayerInteractionController)?.IsTableMode ?? false;
        }

        public override void Interact(IInteractor interactor)
        {
            var p = interactor as PlayerInteractionController;
            if (p == null) return;

            if (!IsLifted) Pickup(p);
            else Drop();
        }

        public new abstract void EnableCollider(bool state);
    }
}
