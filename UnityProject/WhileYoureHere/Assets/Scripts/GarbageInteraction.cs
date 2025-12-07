using Interactable;
using UnityEngine;

public class GarbageInteraction : InteractableBehaviour
{
    [SerializeField] BroomScript broom;

    public override void Interact(IInteractor interactor)
    {
        if (!broom.IsHolding)
        {
            Debug.LogWarning("You need to hold the broom to pick up the garbage!");
            return;
        }
        Destroy(gameObject);
    }
}
