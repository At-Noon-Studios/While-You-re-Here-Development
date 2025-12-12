using UnityEngine;
using Interactable;
using Interactable.Holdable;

public class GarbageInteraction : InteractableBehaviour
{
    [SerializeField] BroomScript broom;

    public override void Interact(IInteractor interactor)
    {
        if (broom.IsBroomBeingHeld)
        {
            Destroy(gameObject);
        }
        else if (!broom.IsBroomBeingHeld)
        {
            return;
        }
    }
}