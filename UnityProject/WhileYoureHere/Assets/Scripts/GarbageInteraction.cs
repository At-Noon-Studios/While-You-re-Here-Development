using UnityEngine;
using Interactable;
using Interactable.Holdable;

public class GarbageInteraction : InteractableBehaviour
{
    [SerializeField] BroomScript broom;

    public override void Interact(IInteractor interactor)
    {
        // Debug.Log("Is held status is: " + broom.IsBroomBeingHeld);
        if (broom.IsBroomBeingHeld)
        {
            // Debug.Log("Holding da garbage!");
            Destroy(gameObject);
        }
        else if (!broom.IsBroomBeingHeld)
        {
            Debug.LogWarning("You need to hold the broom to pick up the garbage!");
            Debug.LogWarning("Status of broom is holding variable is: " + broom.IsBroomBeingHeld);
            return;
        }
    }
}