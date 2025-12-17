using cleaningcabin;
using UnityEngine;
using Interactable;

public class Garbage : InteractableBehaviour
{
    [Header("Reference to the broom")]
    [SerializeField] private BroomMovementDetection broom;
    
    [Header("Garbage Config Settings")]
    [SerializeField] private GarbageConfig garbageConfig;
    public override void Interact(IInteractor interactor)
    {
        if (!broom.IsBroomBeingHeld) return;
        AudioManager.instance.PlaySound(garbageConfig.GarbageCollectClip, transform, 1);
        Destroy(gameObject);
    }

    public override string InteractionText(IInteractor interactor) => string.Empty;
}