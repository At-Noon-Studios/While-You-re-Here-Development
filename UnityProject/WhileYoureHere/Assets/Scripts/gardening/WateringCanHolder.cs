using Interactable;
using Interactable.Concrete.ObjectHolder;
using Interactable.Holdable;
using JetBrains.Annotations;
using ScriptableObjects.Interactable;
using UnityEngine;

namespace gardening
{
    public class WateringCanHolder : InteractableBehaviour
    {
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;
        [CanBeNull] private IHoldableObject _heldObject;
        [SerializeField] private Canvas interactionCanvas;

        private void Start()
        {
            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }
    
        public override void Interact(IInteractor interactor)
        {
            if (_heldObject == null)
            {
                _heldObject = interactor.HeldObject;
                _heldObject!.Place(placePoint.position);
                
                if (interactionCanvas != null)
                    interactionCanvas.gameObject.SetActive(false);
                
                return;
            }
            _heldObject.Interact(interactor);
            _heldObject = null;
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            if (_heldObject == null) return interactor.HeldObject is IPlaceable;
            return true;
        }
        
        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            bool isHoldingPlaceable = interactor.HeldObject is IPlaceable;

            if (_heldObject == null && isHoldingPlaceable)
            {
                if (interactionCanvas != null)
                    interactionCanvas.gameObject.SetActive(true);
            }
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

    }
}
