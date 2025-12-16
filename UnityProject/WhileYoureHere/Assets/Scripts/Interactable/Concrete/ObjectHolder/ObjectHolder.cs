using Interactable.Holdable;
using JetBrains.Annotations;
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
            if (_heldObject == null)
            {
                _heldObject = interactor.HeldObject;
                _heldObject!.Place(placePoint.position, Quaternion.Euler(placedObjectRotation), this);
                _heldObject.EnableCollider(false);
                return;
            }
            _heldObject.Interact(interactor);
            _heldObject = null;
        }
        
        public void PlaceHeldObject(HoldableObjectBehaviour obj, Vector3 position, Quaternion rotation)
        {
            _heldObject = obj;
            obj.Place(position, rotation, this);
        }

        public void ClearHeldObject(HoldableObjectBehaviour obj)
        {
            if (_heldObject == obj)
                _heldObject = null;
        }
        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (_heldObject == null) return interactor.HeldObject is IPlaceable;
            return true;
        }

        public override string InteractionText(IInteractor interactor) => _heldObject == null ? "Place " + (interactor.HeldObject?.InteractionText(interactor) ?? "held object") : "Pick up " + _heldObject.InteractionText(interactor);
    }
}
