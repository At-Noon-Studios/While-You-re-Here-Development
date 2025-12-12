using Interactable.Holdable;
using JetBrains.Annotations;
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

        protected override void Awake()
        {
            base.Awake();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
            
            // var player = GameObject.FindWithTag("Player");
            // if (player != null)
            // {
            //     var cam = player.GetComponentInChildren<Camera>();
            //     if (cam != null)
            //         _playerCamera = cam.transform;
            // }
        }

        private void Update()
        {
            if (interactionCanvas != null &&
                interactionCanvas.gameObject.activeSelf &&
                _playerCamera != null)
            {
                interactionCanvas.transform.LookAt(_playerCamera);
                interactionCanvas.transform.Rotate(0f, 180f, 0f);
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

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(canInteract);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
    }
}
