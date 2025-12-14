using Interactable;
using UnityEngine;
using player_controls;
// using Interactable;
using Interactable.Holdable;
using System.Collections;
using cleaningcabin;
using time;
using ScriptableObjects.Events;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class DirtInteractable : InteractableBehaviour
{
    Renderer materialColor;
    [SerializeField] Color[] colorTransition;

    Color secondColor = new(255, 0, 0);
    [SerializeField] BroomScript broom;

    AudioSource audioSource;
    // [SerializeField] SweepingData sweepingData;

    public MovementController movementController;
    public CameraController cameraController;

    public bool isMiniGameActive { get; private set; }

    [SerializeField] Transform minigameStartingPos;
    public float transitionSpeed = 5f;

    [SerializeField] private BroomMovementDetection broomMD;
    // [SerializeField] AudioClip sweepingClip;
    // public AudioClip sweepingData.sweepingClip;

    private IEnumerator stopMovement;

    protected new void Awake()
    {
        base.Awake();
        materialColor = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        // stopMovement = StopMovement(2);
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


    void Update()
    {
        if (!broom.IsBroomBeingHeld || !isMiniGameActive) return;
        else
        {
            StartSweepingMinigame();
        }
    }

    public void StartSweepingMinigame()
    {
        isMiniGameActive = true;
        // materialColor.material.color = secondColor;
        cameraController.canLook = false;
        movementController.canMove = false;
        var camPos = GameObject.FindWithTag("MainCamera");
        var playerPos = GameObject.FindWithTag("Player");

        playerPos.transform.position = Vector3.Lerp(playerPos.transform.position, minigameStartingPos.transform.position, transitionSpeed * Time.deltaTime);
        camPos.transform.rotation = Quaternion.Lerp(camPos.transform.rotation, minigameStartingPos.transform.rotation, transitionSpeed * Time.deltaTime);

        // broomMD.SetBroomRotation();
        broomMD.SetMiniGamePos();
        // playerPos.transform.position = minigameStartingPos.position;
        // camPos.transform.rotation = minigameStartingPos.rotation;

        // StartCoroutine(stopMovement);
    }

    // private IEnumerator StopMovement(float delay)
    // {
    //     yield return new WaitForSeconds(delay);

    //     // Debug.Log("You should be able to move after 2 seconds!");
    //     cameraController.canLook = true;
    //     movementController.canMove = true;
    // }
    
    public override string InteractionText(IInteractor interactor) => string.Empty;
}
