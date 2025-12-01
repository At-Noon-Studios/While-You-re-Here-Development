using Interactable;
using Interactable.Concrete.Key;
using UnityEngine;

namespace door
{
    public class Keyhole : InteractableBehaviour
    {
        [Header("Key Settings")]
        [SerializeField] private Transform keyHolePosition;
        [SerializeField] private Vector3 keyRotation;
        
        public bool IsLocked { get; private set; }
        
        public override void Interact(IInteractor interactor)
        {
            if (interactor.HeldObject is not Key key) return;
            key.Place(keyHolePosition.position, Quaternion.Euler(keyRotation));
            key.EnableCollider(false);
            EnableCollider(false);
            key.StartMinigame(interactor, (newState) =>
            {
                EnableCollider(true);
                IsLocked = newState;
            });
        }

        public override bool InteractableBy(IInteractor interactor)
        {
            return interactor.HeldObject is Key;
        }

        public override string InteractionText(IInteractor interactor)
        {
            return "Operate the lock";
        }
    }
}