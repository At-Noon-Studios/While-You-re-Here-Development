using System;
using System.Linq;
using Interactable.Holdable;
using JetBrains.Annotations;
using making_tea;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public class ObjectHolder : InteractableBehaviour
    {
        [Header("Placement")]
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;

        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        [CanBeNull] private IHoldableObject _heldObject;
        private Transform _playerCamera;

        public event Action<IHoldableObject> OnPlaced;
        public event Action<IHoldableObject> OnRemoved;

        protected override void Awake()
        {
            base.Awake();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            
            var cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
                _playerCamera = cam.transform;
        }

        private void Update()
        {
            if (interactionCanvas == null ||
                !interactionCanvas.gameObject.activeSelf ||
                _playerCamera == null) return;
            
            interactionCanvas.transform.LookAt(_playerCamera);
            interactionCanvas.transform.Rotate(0f, 180f, 0f);
        }

        public override void Interact(IInteractor interactor)
        {
            var pic = interactor as PlayerInteractionController;

            if (pic != null && pic.IsTableMode)
            {
                HandleTableModePlacement();
                return;
            }

            if (_heldObject != null)
                return;

            if (interactor.HeldObject == null)
                return;

            _heldObject = interactor.HeldObject;

            _heldObject.Place(placePoint.position,
                              Quaternion.Euler(placedObjectRotation),
                              this);

            var go = ((MonoBehaviour)_heldObject).gameObject;
            go.transform.SetParent(placePoint);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(placedObjectRotation);

            OnPlaced?.Invoke(_heldObject);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public void ClearHeldObject(IHoldableObject obj)
        {
            if (_heldObject != obj) return;
            OnRemoved?.Invoke(obj);

            _heldObject = null;
        }

        private void HandleTableModePlacement()
        {
            var pickups = FindObjectsByType<TablePickup>(FindObjectsSortMode.None);
            var pickup = pickups.FirstOrDefault(p => p.IsTableHeld);
            if (pickup == null) return;

            var obj = pickup.gameObject;
            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            obj.transform.SetParent(null);
            obj.transform.position = placePoint.position;
            obj.transform.rotation = Quaternion.Euler(placedObjectRotation);

            pickup.ForceDropFromTableMode();
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            if (interactor is PlayerInteractionController { IsTableMode: true })
                return true;

            return _heldObject == null &&
                   interactor.HeldObject is IPlaceable;
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            var canInteract = _heldObject == null &&
                              interactor.HeldObject is IPlaceable;

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(canInteract);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }
    }
}
