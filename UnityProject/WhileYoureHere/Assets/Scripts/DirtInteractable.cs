using Interactable;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class DirtInteractable : InteractableBehaviour
{
    Renderer materialColor;
    [SerializeField] Color[] colorTransition;

    Color secondColor = new(255, 0, 0);
    [SerializeField] BroomScript broom;

    AudioSource audioSource;
    [SerializeField] AudioClip sweepingClip;

    protected new void Awake()
    {
        base.Awake();
        materialColor = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsHolding)
        {
            Debug.Log("Broom holding boolean is: " + broom.IsHolding);
            Debug.Log("You need to hold the broom for this!");
            return;
        }
        else if (broom.IsHolding)
        {
            Debug.Log("Broom holding boolean is: " + broom.IsHolding);
            Debug.Log("How nice of you to hold the broom before interacting!");
            // materialColor.material.color = secondColor;
            ColorTransition();
            audioSource.PlayOneShot(sweepingClip);
        }
        Debug.Log("You just interacted with a pile of shit... gross...");
    }

    public void ColorTransition()
    {
        for (int i = 0; i < colorTransition.Length; i++)
        {
            materialColor.material.color = colorTransition[i];
        }
    }
}
