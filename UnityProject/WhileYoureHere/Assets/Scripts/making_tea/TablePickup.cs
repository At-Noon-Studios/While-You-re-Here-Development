using Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public abstract class TablePickup : InteractableBehaviour, ITablePickup
    {
        [SerializeField] private Canvas interactionCanvasPrimary;
        [SerializeField] private Canvas interactionCanvasSecondary;

        [Header("Table Settings")]
        [SerializeField] private LayerMask tableLayer;
        [SerializeField] private float liftHeight = 0.15f;

        protected bool IsLifted;
        protected PlayerInteractionController Pic;

        private Rigidbody _rb;
        private Transform _playerCamera;

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

            if (!IsLifted || Pic == null || !Pic.IsTableMode)
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

            if (interactionCanvasSecondary != null && interactionCanvasSecondary.gameObject.activeSelf)
            {
                interactionCanvasSecondary.transform.LookAt(_playerCamera);
                interactionCanvasSecondary.transform.Rotate(0f, 180f, 0f);
            }
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
            ShowSecondaryUI();
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
            ShowPrimaryUI();
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

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            var p = interactor as PlayerInteractionController;
            if (p == null || !p.IsTableMode) return;

            if (!IsLifted) ShowPrimaryUI();
            else ShowSecondaryUI();
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (!IsLifted)
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

        public new abstract void EnableCollider(bool state);
    }
}
