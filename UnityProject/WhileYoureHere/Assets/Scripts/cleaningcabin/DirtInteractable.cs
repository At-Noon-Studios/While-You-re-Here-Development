using Interactable;
using UnityEngine;
using player_controls;
// using Interactable;
using Interactable.Holdable;
using System.Collections;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class DirtInteractable : InteractableBehaviour
{
    Renderer materialColor;
    [SerializeField] Color[] colorTransition;

    Color secondColor = new(255, 0, 0);
    [SerializeField] BroomScript broom;

    AudioSource audioSource;
    [SerializeField] SweepingData sweepingData;

    public MovementController movementController;
    public CameraController cameraController;

    // [SerializeField] AudioClip sweepingClip;
    // public AudioClip sweepingData.sweepingClip;

    private IEnumerator stopMovement;

    protected new void Awake()
    {
        base.Awake();
        materialColor = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        stopMovement = StopMovement(2);
    }

    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsBroomBeingHeld)
        {
            Debug.Log("You need to hold the broom for this!");
            return;
        }
        else if (broom.IsBroomBeingHeld)
        {
            // audioSource.PlayOneShot(sweepingData.SweepingClip);
            Debug.Log("How nice of you to hold the broom before interacting!");
            StartSweepingMinigame();
        }
        Debug.Log("You just interacted with a pile of shit... gross...");
    }

    // public void ColorTransition()
    // {
    //     for (int i = 0; i < colorTransition.Length; i++)
    //     {
    //         materialColor.material.color = colorTransition[i];
    //     }
    // }

    public void StartSweepingMinigame()
    {
        materialColor.material.color = secondColor;
        // ColorTransition();
        cameraController.canLook = false;
        movementController.canMove = false;
        Debug.Log("Sweeping has started, everythings paused!");

        StartCoroutine(stopMovement);
    }

    private IEnumerator StopMovement(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Debug.Log("You should be able to move after 2 seconds!");
        cameraController.canLook = true;
        movementController.canMove = true;
    }
}
