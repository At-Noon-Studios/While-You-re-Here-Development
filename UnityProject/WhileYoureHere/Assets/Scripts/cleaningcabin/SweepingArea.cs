using System.Collections;
using Interactable;
using UnityEngine;
using player_controls;
using cleaningcabin;
using PlayerControls;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class SweepingArea : InteractableBehaviour
{
    private Renderer _materialColor;
    [SerializeField] Color[] colorTransition;

    private Color _secondColor = new(255, 0, 0);
    [SerializeField] BroomMovementDetection broom;

    AudioSource _audioSource;
    [SerializeField] private AudioClip popClip;
    public MovementController movementController;
    public CameraController cameraController;

    public bool IsMiniGameActive { get; private set; }
    
    [SerializeField] Transform minigameStartingPos;
    public float transitionSpeed = 0.004f;

    [SerializeField] private BroomMovementDetection broomMD;

    public float time;
    private IEnumerator _coroutine;
    
    public GameObject playerPos;
    public GameObject camPos;
    
    // Vector3 attempt1;
    // Quaternion attempt2;

    private Material _material;
    protected new void Awake()
    {
        base.Awake();
        _materialColor = GetComponent<Renderer>();
        _audioSource = GetComponent<AudioSource>();
        camPos = GameObject.FindWithTag("MainCamera");
        playerPos = GameObject.FindWithTag("Player");
        IsMiniGameActive = false;
        // attempt1 = new Vector3(-1, 0, -1);
        // attempt2 = new Quaternion(-60, 0, 0, 0);
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
        // Debug.Log("Status of mini-game is: " + IsMiniGameActive);
        if (!broom.IsBroomBeingHeld || !IsMiniGameActive) return;
        
        StartSweepingMinigame();
    }

    private void StartSweepingMinigame()
    {
        IsMiniGameActive = true;
        cameraController.PauseCameraMovement();
        movementController.PauseMovement();
        
        playerPos.transform.position = Vector3.Lerp(playerPos.transform.position,
            minigameStartingPos.transform.position, transitionSpeed * Time.deltaTime);
        camPos.transform.rotation = Quaternion.Lerp(camPos.transform.rotation, minigameStartingPos.transform.rotation,
            transitionSpeed * Time.deltaTime);

        broomMD.SetMiniGameStartPos();
        // time += Time.deltaTime;
        // if (time >= 4 && time <= 5)
        // {
        //     EndSweepingMinigame();
        //     time = 0;
        // }
    }

    public void EndSweepingMinigame()
    {
        IsMiniGameActive = false;
        broomMD.ResetMiniGamePos();
        cameraController.ResumeCameraMovement();
        movementController.ResumeMovement();
        AudioManager.instance.PlaySound(popClip, transform, 1);
        Destroy(gameObject);
        // playerPos.transform.position = Vector3.Lerp(playerPos.transform.position, attempt1, transitionSpeed * Time.deltaTime);
        // camPos.transform.rotation = Quaternion.Lerp(camPos.transform.rotation, attempt2, transitionSpeed * Time.deltaTime);
        // Debug.Log("SweepingMinigame has ended!");
    }
    
    public override string InteractionText(IInteractor interactor) => string.Empty;
}