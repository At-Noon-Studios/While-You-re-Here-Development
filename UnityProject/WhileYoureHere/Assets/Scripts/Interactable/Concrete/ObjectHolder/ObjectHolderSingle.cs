using System;
using System.Linq;
using Interactable.Holdable;
using JetBrains.Annotations;
using making_tea;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public class ObjectHolderSingle : InteractableBehaviour, IObjectHolder
    {
        [Header("Placement")]
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;

        [CanBeNull] private IHoldableObject _heldObject;

        public event Action<IHoldableObject> OnPlaced;
        public event Action<IHoldableObject> OnRemoved;

        protected override void Awake()
        {
            base.Awake();
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

            _heldObject.Place(
                placePoint.position,
                Quaternion.Euler(placedObjectRotation),
                this
            );

            var go = ((MonoBehaviour)_heldObject).gameObject;
            go.transform.SetParent(placePoint);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(placedObjectRotation);

            OnPlaced?.Invoke(_heldObject);
        }

        public void ClearHeldObject(GameObject obj)
        {
            if ((_heldObject as Component)?.gameObject != obj) return;

            OnRemoved?.Invoke(_heldObject);
            _heldObject = null;
        }

        private void HandleTableModePlacement()
        {
            var pickups = FindObjectsByType<TablePickup>(FindObjectsSortMode.None);
            var pickup = pickups.FirstOrDefault(p => p.IsTableHeld);
            if (pickup == null) return;

            var obj = pickup.gameObject;

            pickup.ForceDropFromTableMode();

            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            obj.transform.SetParent(null);
            obj.transform.position = placePoint.position;
            obj.transform.rotation = Quaternion.Euler(placedObjectRotation);
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (interactor is PlayerInteractionController { IsTableMode: true })
                return true;

            return _heldObject == null &&
                   interactor.HeldObject is IPlaceable;
        }
        
    }
}
