using Interactable;
using Interactable.Concrete.Key;
using UnityEngine;

namespace door
{
    public class Keyhole : InteractableBehaviour
    {
        [Header("Door Lock")]
        [SerializeField] private bool isLocked = false;
        public bool IsLocked => isLocked;
        [Header("Key Settings")]
        [SerializeField] private Transform keyHolePosition;
        [SerializeField] private Vector3 keyRotation;
        
        public override void Interact(IInteractor interactor)
        {
            if (interactor.HeldObject is not Key key) return;
            key.Place(keyHolePosition.position, Quaternion.Euler(keyRotation));
            key.StartMinigame(interactor, (newState) => isLocked = newState);
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