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
    
    [SerializeField] private Canvas broomInteractionCanvas;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip popClip;

    public MovementController movementController;
    public CameraController cameraController;
    public bool IsMiniGameActive { get; private set; }
    
    [SerializeField] Transform minigameStartingPos;
    public float transitionSpeed = 0.004f;

    private GameObject _playerPos;
    private GameObject _camPos;

    private Material _material;

    // private float _lerpSpeed = 1.0f;
    protected new void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
        _camPos = GameObject.FindWithTag("MainCamera");
        _playerPos = GameObject.FindWithTag("Player");
        IsMiniGameActive = false;
        // broomInteractionCanvas.gameObject.SetActive(true);
    }
    
    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsBroomBeingHeld || IsMiniGameActive) return;
        // if (!broom.IsBroomBeingHeld) return;
        StartSweepingMinigame();
    }

    void Update()
    {
        if (!IsMiniGameActive) return;
        _playerPos.transform.position = Vector3.Lerp(_playerPos.transform.position,
            minigameStartingPos.transform.position, transitionSpeed * Time.deltaTime);
        _camPos.transform.rotation = Quaternion.Lerp(_camPos.transform.rotation, minigameStartingPos.transform.rotation,
            transitionSpeed * Time.deltaTime);
    }

    private void StartSweepingMinigame()
    {
        IsMiniGameActive = true;
        cameraController.PauseCameraMovement();
        movementController.PauseMovement();
        if (broomInteractionCanvas != null) { broomInteractionCanvas.gameObject.SetActive(false);}
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