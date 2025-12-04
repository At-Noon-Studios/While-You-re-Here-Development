using Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public abstract class TablePickup : MonoBehaviour, ITablePickup
    {
        protected bool Lifted;
        protected PlayerInteractionController Pic;
        private Rigidbody _rb;

        private const float FixedLiftHeight = 1.5f;
        private const float FixedRayDistance = 10f;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public bool IsTableHeld => Lifted;

        public virtual void Pickup(PlayerInteractionController p)
        {
            Lifted = true;
            Pic = p;

            if (_rb) _rb.isKinematic = true;
            EnableCollider(false);
        }

        public virtual void Drop()
        {
            Lifted = false;

            if (_rb) _rb.isKinematic = false;
            EnableCollider(true);
        }

        public virtual void ForceDropFromTableMode() => Drop();

        public virtual string InteractionText(IInteractor i)
            => Lifted ? "Drop" : "Pick up";

        public virtual bool InteractableBy(IInteractor i)
            => (i as PlayerInteractionController)?.TableMode ?? false;

        public virtual void Interact(IInteractor i)
        {
            var p = i as PlayerInteractionController;
            if (p == null) return;

            if (!Lifted) Pickup(p);
            else Drop();
        }

        public abstract void EnableCollider(bool s);

        public virtual void OnHoverEnter(IInteractor i) { }
        public virtual void OnHoverExit(IInteractor i) { }

        protected virtual void Update()
        {
            if (!Lifted || Pic == null || !Pic.TableMode)
                return;

            FollowMouse();
        }

        private void FollowMouse()
        {
            var cam = Pic.PlayerCamera;
            var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, FixedRayDistance)) return;
            var pos = hit.point;
            pos.y = FixedLiftHeight;
            transform.position = pos;
        }
    }
}
