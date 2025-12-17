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

    // private AudioSource _audioSource;
    // [SerializeField] private AudioClip popClip;
    // public float transitionSpeed = 1f;

    [SerializeField] private SweepingAreaConfig sweepingAreaConfig;
    public MovementController movementController;
    public CameraController cameraController;
    public bool IsMiniGameActive { get; private set; }
    
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