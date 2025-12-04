using System.Linq;
using Interactable.Holdable;
using JetBrains.Annotations;
using making_tea;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public class ObjectHolder : InteractableBehaviour
    {
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;

        [CanBeNull] private IHoldableObject _heldObject;

        public override void Interact(IInteractor interactor)
        {
            var pic = interactor as PlayerInteractionController;

            if (pic != null && pic.TableMode)
            {
                HandleTableModePlacement();
                return;
            }

            if (_heldObject == null)
            {
                _heldObject = interactor.HeldObject;

                if (_heldObject == null)
                    return;

                _heldObject.Place(placePoint.position, Quaternion.Euler(placedObjectRotation));
                var go = ((MonoBehaviour)_heldObject).gameObject;
                go.transform.SetParent(placePoint);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(placedObjectRotation);

                return;
            }

            _heldObject.Interact(interactor);
            _heldObject = null;
        }


        private void HandleTableModePlacement()
        {
            var pickups = FindObjectsByType<TablePickup>(FindObjectsSortMode.None);

            var pickup = pickups.FirstOrDefault(p => p.IsTableHeld);
            if (pickup == null)
                return;

            var obj = pickup.gameObject;
            var rb = obj.GetComponent<Rigidbody>();

            if (rb != null)
                rb.isKinematic = true;

            obj.transform.SetParent(null);
            obj.transform.position = placePoint.position;
            obj.transform.rotation = Quaternion.Euler(placedObjectRotation);

            pickup.ForceDropFromTableMode();
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            if (interactor is PlayerInteractionController pic && pic.TableMode)
                return true;

            if (_heldObject == null)
                return interactor.HeldObject is IPlaceable;

            return true;
        }

        public override string InteractionText(IInteractor interactor)
        {
            if (interactor is PlayerInteractionController pic && pic.TableMode)
                return "Place Object";

            return _heldObject == null
                ? "Place " + (interactor.HeldObject?.InteractionText(interactor) ?? "held object")
                : "Pick up " + _heldObject.InteractionText(interactor);
        }
    }
}
