using System.Collections.Generic;
using System.Linq;
using Interactable.Holdable;
using JetBrains.Annotations;
using make_a_fire;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public class ObjectHolder : InteractableBehaviour
    {
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;
        // [CanBeNull] private IHoldableObject _heldObject;
          [SerializeField] private List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
        private readonly List<PlacedObjectData> _placedObjectsInHolders = new List<PlacedObjectData>();
    
        public override void Interact(IInteractor interactor)
        {
            var heldObject = interactor.HeldObject;
            if (heldObject == null) return;

            var heldGameObject = (heldObject as Component)?.gameObject;
            if (!heldGameObject) return;
            
            var placedData = placedObjects.FirstOrDefault(e => e.objectPrefab.GetType() == heldGameObject.GetType());
            if (placedData != null)
            {
                placedObjects.Remove(placedData);
                heldObject.Place(placePoint.position, Quaternion.Euler(placedData.placedObjectRotation));
                _placedObjectsInHolders.Add(placedData);
            }
            
            interactor.SetHeldObject(null);
        }

        // public override bool IsInteractableBy(IInteractor interactor)
        // {
        //     if (_heldObject == null) return interactor.HeldObject is IPlaceable;
        //     return true;
        // }
        
        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;

            var heldObject = interactor.HeldObject;
            if (heldObject == null) return false;

            var heldGameObject = (heldObject as Component)?.gameObject;
            if (!heldGameObject) return false;

            return placedObjects.Any(e => e.objectPrefab.GetType() == heldGameObject.GetType());
        }


        // public override string InteractionText(IInteractor interactor) => _heldObject == null ? "Place " + (interactor.HeldObject?.InteractionText(interactor) ?? "held object") : "Pick up " + _heldObject.InteractionText(interactor);
        
        public override string InteractionText(IInteractor interactor)
        {
            if (!IsInteractableBy(interactor))
                return string.Empty;

            return "Place " + interactor.HeldObject.InteractionText(interactor);
        }

        
    }
}
