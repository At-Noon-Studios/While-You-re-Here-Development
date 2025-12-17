using Interactable;
using UnityEngine;
using player_controls;
using cleaningcabin;
using PlayerControls;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class SweepingArea : InteractableBehaviour
{
    [Header("Broom Script Reference")]
    [SerializeField] private BroomMovementDetection broom;

    [Header("Sweeping Area Config Data")]
    [SerializeField] private SweepingAreaConfig sweepingAreaConfig;
    
    [Header("Camera and Movement-controllers")]
    public MovementController movementController;
    public CameraController cameraController;
    
    [Header("Is the mini game active or not")]
    public bool IsMiniGameActive { get; private set; }
    
    [Header("Position of the starting point of the mini-game")]
    [SerializeField] private Transform minigameStartingPos;

    private GameObject _playerPos;
    private GameObject _camPos;
    
    protected new void Awake()
    {
        base.Awake();
        // _audioSource = GetComponent<AudioSource>();
        _camPos = GameObject.FindWithTag("MainCamera");
        _playerPos = GameObject.FindWithTag("Player");
        IsMiniGameActive = false;
    }
    
    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsBroomBeingHeld || IsMiniGameActive) return;
        StartSweepingMinigame();
    }

    void Update()
    {
        if (!IsMiniGameActive) return;
        _playerPos.transform.position = Vector3.Lerp(_playerPos.transform.position,
            minigameStartingPos.transform.position, sweepingAreaConfig.TransitionSpeed * Time.deltaTime);
        _camPos.transform.rotation = Quaternion.Lerp(_camPos.transform.rotation, minigameStartingPos.transform.rotation,
            sweepingAreaConfig.TransitionSpeed * Time.deltaTime);
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
        AudioManager.instance.PlaySound(sweepingAreaConfig.DestroyAreaClip, transform, 1);
        Destroy(gameObject);
    }
    
    public override string InteractionText(IInteractor interactor) => string.Empty;
}