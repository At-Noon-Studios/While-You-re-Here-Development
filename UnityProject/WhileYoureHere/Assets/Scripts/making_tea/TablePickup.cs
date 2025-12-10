using Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public abstract class TablePickup : InteractableBehaviour, ITablePickup
    {
        [SerializeField] private Canvas interactionCanvasPrimary;
        [SerializeField] private Canvas interactionCanvasSecondary;

        protected bool Lifted;
        protected PlayerInteractionController Pic;

        private Rigidbody _rb;
        private Transform _playerCamera;

        private const float FixedLiftHeight = 1.5f;
        private const float FixedRayDistance = 10f;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();

            if (interactionCanvasPrimary != null)
                interactionCanvasPrimary.gameObject.SetActive(false);

            if (interactionCanvasSecondary != null)
                interactionCanvasSecondary.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerCamera = player.GetComponentInChildren<Camera>()?.transform;
        }

        protected virtual void Update()
        {
            RotateUI();

            if (!Lifted || Pic == null || !Pic.TableMode)
                return;

            FollowMouse();
        }

        private void RotateUI()
        {
            if (_playerCamera == null) return;

            if (interactionCanvasPrimary != null && interactionCanvasPrimary.gameObject.activeSelf)
            {
                interactionCanvasPrimary.transform.LookAt(_playerCamera);
                interactionCanvasPrimary.transform.Rotate(0f, 180f, 0f);
            }

            if (interactionCanvasSecondary == null || !interactionCanvasSecondary.gameObject.activeSelf) return;
            
            interactionCanvasSecondary.transform.LookAt(_playerCamera);
            interactionCanvasSecondary.transform.Rotate(0f, 180f, 0f);
        }

        private void FollowMouse()
        {
            var cam = Pic.PlayerCamera;
            var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, FixedRayDistance))
                return;

            var pos = hit.point;
            pos.y = FixedLiftHeight;
            transform.position = pos;
        }

        public bool IsTableHeld => Lifted;

        public virtual void Pickup(PlayerInteractionController p)
        {
            Lifted = true;
            Pic = p;

            if (_rb != null)
                _rb.isKinematic = true;

            EnableCollider(false);
            ShowSecondaryUI();
        }

        public virtual void Drop()
        {
            Lifted = false;
            Pic = null;

            if (_rb != null)
                _rb.isKinematic = false;

            EnableCollider(true);
            ShowPrimaryUI();
        }

        public virtual void ForceDropFromTableMode() => Drop();

        public override string InteractionText(IInteractor i) => string.Empty;

        public override bool InteractableBy(IInteractor i)
        {
            return (i as PlayerInteractionController)?.TableMode ?? false;
        }

        public override void Interact(IInteractor i)
        {
            var p = i as PlayerInteractionController;
            if (p == null) return;

            if (!Lifted) Pickup(p);
            else Drop();
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            var p = interactor as PlayerInteractionController;
            if (p == null || !p.TableMode) return;

            if (!Lifted) ShowPrimaryUI();
            else ShowSecondaryUI();
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (!Lifted)
                HideAllUI();
        }

        private void ShowPrimaryUI()
        {
            if (interactionCanvasPrimary != null)
                interactionCanvasPrimary.gameObject.SetActive(true);

            if (interactionCanvasSecondary != null)
                interactionCanvasSecondary.gameObject.SetActive(false);
        }

        private void ShowSecondaryUI()
        {
            if (interactionCanvasPrimary != null)
                interactionCanvasPrimary.gameObject.SetActive(false);

            if (interactionCanvasSecondary != null)
                interactionCanvasSecondary.gameObject.SetActive(true);
        }

        private void HideAllUI()
        {
            if (interactionCanvasPrimary != null)
                interactionCanvasPrimary.gameObject.SetActive(false);
        
            if (interactionCanvasSecondary != null)
                interactionCanvasSecondary.gameObject.SetActive(false);
        }

        public abstract override void EnableCollider(bool state);
    }
}
