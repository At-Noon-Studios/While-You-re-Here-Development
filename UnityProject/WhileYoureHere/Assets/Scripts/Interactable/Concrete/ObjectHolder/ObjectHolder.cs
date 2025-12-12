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
        [SerializeField] private List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
        private readonly List<PlacedObjectData> _placedObjectsInHolders = new List<PlacedObjectData>();
        
        
        public override void Interact(IInteractor interactor)
        {
            var heldObject = interactor.HeldObject;
            if (heldObject == null) return;

            var heldGameObject = (heldObject as Component)?.gameObject;
            if (!heldGameObject) return;
            
            var placedData = placedObjects.FirstOrDefault(e => e.objectPrefab == heldGameObject);
            if (placedData != null)
            {
                heldObject.Place(placePoint.position, Quaternion.Euler(placedData.placedObjectRotation));
                _placedObjectsInHolders.Add(placedData);
            }
            
            interactor.SetHeldObject(null);
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            return placedObjects != null && placedObjects.Any(p => p.objectPrefab);
        }
        
        public override string InteractionText(IInteractor interactor)
        {
            if (interactor?.HeldObject == null) return null;
            
            string heldText = interactor.HeldObject.InteractionText(interactor);
            return "Place " + (string.IsNullOrEmpty(heldText) ? "held object" : heldText);
        }
    }
}