using System.Collections;
using Interactable;
using UnityEngine;
using player_controls;
using cleaningcabin;
using NUnit.Framework;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class SweepingArea : InteractableBehaviour
{
    private Renderer _materialColor;
    [SerializeField] Color[] colorTransition;

    private Color _secondColor = new(255, 0, 0);
    [SerializeField] BroomMovementDetection broom;

    AudioSource _audioSource;

    public MovementController movementController;
    public CameraController cameraController;

    public bool IsMiniGameActive { get; private set; }
    
    [SerializeField] Transform minigameStartingPos;
    public float transitionSpeed = 5f;

    [SerializeField] private BroomMovementDetection broomMD;

    public float time;
    private IEnumerator _coroutine;
    
    public GameObject playerPos;
    public GameObject camPos;
    public Quaternion camPosDefaultPos;
    public Vector3 playerPosDefaultPos;
    protected new void Awake()
    {
        base.Awake();
        _materialColor = GetComponent<Renderer>();
        _audioSource = GetComponent<AudioSource>();
        camPos = GameObject.FindWithTag("MainCamera");
        camPosDefaultPos = camPos.transform.rotation;
        playerPos = GameObject.FindWithTag("Player");
        playerPosDefaultPos = playerPos.transform.position;
        IsMiniGameActive = false;
    }

    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsBroomBeingHeld || IsMiniGameActive)
        {
            Debug.Log("You need to hold the broom for this!");
            return;
        }
        if (broom.IsBroomBeingHeld)
        {
            // _audioSource.PlayOneShot(sweepingData.SweepingClip);
            // Debug.Log("How nice of you to hold the broom before interacting!");
            StartSweepingMinigame();
        }
    }

    void Update()
    {
        Debug.Log("Status of mini-game is: " + IsMiniGameActive);
        if (!broom.IsBroomBeingHeld || !IsMiniGameActive) return;
        
        StartSweepingMinigame();
    }

    private void StartSweepingMinigame()
    {
        IsMiniGameActive = true;
        cameraController.canLook = false;
        movementController.canMove = false;

        playerPos.transform.position = Vector3.Lerp(playerPos.transform.position,
            minigameStartingPos.transform.position, transitionSpeed * Time.deltaTime);
        camPos.transform.rotation = Quaternion.Lerp(camPos.transform.rotation, minigameStartingPos.transform.rotation,
            transitionSpeed * Time.deltaTime);

        broomMD.SetMiniGameStartPos();
        time += Time.deltaTime;
        if (time >= 2 && time <= 3)
        {
            EndSweepingMinigame();
            time = 0;
        }
    }

    private void EndSweepingMinigame()
    {
        IsMiniGameActive = false;
        broomMD.ResetMiniGamePos();
        cameraController.canLook = true;
        movementController.canMove = true;
        playerPos.transform.position = playerPosDefaultPos;
        camPos.transform.rotation = camPosDefaultPos;
        // playerPos.transform.position = Vector3.Lerp(playerPosDefaultPos, playerPos.transform.position, transitionSpeed * Time.deltaTime);
        // camPos.transform.rotation = Quaternion.Lerp(camPosDefaultPos, camPos.transform.rotation, transitionSpeed * Time.deltaTime);
        Debug.Log("SweepingMinigame has ended!");
    }
    
    public override string InteractionText(IInteractor interactor) => string.Empty;
}