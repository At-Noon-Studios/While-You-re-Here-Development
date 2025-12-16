using Interactable;
using UnityEngine;
using player_controls;
using cleaningcabin;
using PlayerControls;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class SweepingArea : InteractableBehaviour
{
    [SerializeField] private BroomMovementDetection broom;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip popClip;

    public MovementController movementController;
    public CameraController cameraController;
    public bool IsMiniGameActive { get; private set; }
    
    [SerializeField] Transform minigameStartingPos;
    public float transitionSpeed = 0.004f;

    public GameObject playerPos;
    public GameObject camPos;

    private Material _material;
    protected new void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
        camPos = GameObject.FindWithTag("MainCamera");
        playerPos = GameObject.FindWithTag("Player");
        IsMiniGameActive = false;
    }
    
    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsBroomBeingHeld || IsMiniGameActive) return;
        // {
        //     Debug.Log("You need to hold the broom for this!");
        //     return;
        // }
        if (broom.IsBroomBeingHeld)
        {
            StartSweepingMinigame();
        }
    }

    void Update()
    {
        if (!IsMiniGameActive) return;
        // Transition schmoovly
        playerPos.transform.position = Vector3.Lerp(playerPos.transform.position,
            minigameStartingPos.transform.position, transitionSpeed * Time.deltaTime);
        camPos.transform.rotation = Quaternion.Lerp(camPos.transform.rotation, minigameStartingPos.transform.rotation,
            transitionSpeed * Time.deltaTime);
    }

    private void StartSweepingMinigame()
    {
        IsMiniGameActive = true;
        cameraController.PauseCameraMovement();
        movementController.PauseMovement();
        broom.SetMiniGameStartPos();
    }

    public void EndSweepingMinigame()
    {
        IsMiniGameActive = false;
        broom.ResetMiniGamePos();
        cameraController.ResumeCameraMovement();
        movementController.ResumeMovement();
        AudioManager.instance.PlaySound(popClip, transform, 1);
        Destroy(gameObject);
    }
    
    public override string InteractionText(IInteractor interactor) => string.Empty;
}