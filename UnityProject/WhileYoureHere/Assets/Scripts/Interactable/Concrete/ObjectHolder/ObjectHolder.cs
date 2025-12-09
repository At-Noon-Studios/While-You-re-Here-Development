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
        [SerializeField] private List<PlacedObjectData> placedObjects;
        private readonly List<PlacedObjectData> _placedObjectsInHolders;
        
        public override void Interact(IInteractor interactor)
        {
            var heldObject = interactor.HeldObject;
            if (heldObject == null) return;

            var placedData = placedObjects.FirstOrDefault(e =>
                e.objectPrefab == (heldObject as Component)?.gameObject
            );

            if (placedData != null)
            {
                heldObject.Place(placePoint.position, Quaternion.Euler(placedData.placedObjectRotation));
                _placedObjectsInHolders.Add(placedData);
            }

            interactor.SetHeldObject(null);
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            return interactor.HeldObject is IPlaceable placeable &&
                   placedObjects.Any(p =>
                       p.objectPrefab == (placeable as Component)?.gameObject
                   );
        }
        
        public override string InteractionText(IInteractor interactor) => interactor.HeldObject != null
                ? "Place " + (interactor.HeldObject.InteractionText(interactor) ?? "held object") : null;
    }
}