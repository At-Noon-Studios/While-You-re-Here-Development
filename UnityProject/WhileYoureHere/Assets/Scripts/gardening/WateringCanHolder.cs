using Interactable;
using Interactable.Concrete.ObjectHolder;
using Interactable.Holdable;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace gardening
{
    public class WateringCanHolder : InteractableBehaviour
    {
        [Header("Placement")]
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;

        [Header("Interaction UI")]
        [SerializeField] private Image interactionImage;

        [CanBeNull] private IHoldableObject _heldObject;
        private Transform _playerCamera;

        protected override void Awake()
        {
            base.Awake();

            if (interactionImage != null)
                interactionImage.enabled = false;
        }

        private void Update()
        {
            if (interactionImage != null &&
                interactionImage.gameObject.activeSelf &&
                _playerCamera != null)
            {
                interactionImage.transform.LookAt(_playerCamera);
                interactionImage.transform.Rotate(0f, 180f, 0f);
            }
        }

        public override void Interact(IInteractor interactor)
        {
            if (_heldObject == null)
            {
                if (interactor.HeldObject == null) return;

                _heldObject = interactor.HeldObject;
                _heldObject.Place(placePoint.position, Quaternion.Euler(placedObjectRotation), this);
                return;
            }

            _heldObject.Interact(interactor);
            _heldObject = null;
        }
        
        public void ClearHeldObject(IHoldableObject obj)
        {
            if (_heldObject == obj)
                _heldObject = null;
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            return _heldObject == null &&
                   interactor.HeldObject is IPlaceable;
        }
        
        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            bool canInteract = _heldObject == null &&
                               interactor.HeldObject is IPlaceable;

            if (interactionImage != null)
                interactionImage.enabled = canInteract;
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionImage != null)
                interactionImage.enabled = false;
        }

        public override string InteractionText(IInteractor interactor)
        {
           return string.Empty;
        }
    }
}