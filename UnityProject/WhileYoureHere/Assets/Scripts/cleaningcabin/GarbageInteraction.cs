using UnityEngine;
using Interactable;
using Interactable.Holdable;

public class GarbageInteraction : InteractableBehaviour
{
    [SerializeField] BroomScript broom;
    // private AudioSource _audioSource;
    [SerializeField] AudioClip garbageClip;

    public override void Interact(IInteractor interactor)
    {
        if (broom.IsBroomBeingHeld)
        {
            AudioManager.instance.PlaySound(garbageClip, transform, 1);
            Destroy(gameObject);
        }
        else if (!broom.IsBroomBeingHeld)
        {
            return;
        }
    }

    public override string InteractionText(IInteractor interactor) => "";

}