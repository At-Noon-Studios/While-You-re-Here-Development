using System;
using Interactable;
using UnityEngine;
using player_controls;
using cleaningcabin;
using PlayerControls;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class SweepingArea : InteractableBehaviour
{
    [Header("Broom Script Reference")]
    [SerializeField] private BroomMovementDetection broom;

    [SerializeField] private Transform broomModel;
    [SerializeField] private BroomConfig broomConfig;
    private float _sweepingTimeInSeconds; 
    private float _broomXPos;
    private Material _sweepingColor;
    private Color _endingSweepingColor;
    
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
    private Material _areaStartingColor;
    
    protected new void Awake()
    {
        base.Awake();
        _camPos = GameObject.FindWithTag("MainCamera");
        _playerPos = GameObject.FindWithTag("Player");
        IsMiniGameActive = false;
        _sweepingColor = GetComponent<Renderer>().material;
        _endingSweepingColor = Color.white;
        if (broomModel != null)
        {
            _broomXPos = broomModel.localPosition.x;
        }
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
        broom.SetActiveArea(this);
        broom.SetMiniGameStartPos();
    }

    public void EndSweepingMinigame()
    {
        IsMiniGameActive = false;
        broom.ClearActiveArea();
        broom.ResetMiniGamePos();
        cameraController.ResumeCameraMovement();
        movementController.ResumeMovement();
        AudioManager.instance.PlaySound(sweepingAreaConfig.DestroyAreaClip, transform, 1);
        Destroy(gameObject);
    }

    public void CleanArea(InputValue inputValue)
    {
        if (!IsMiniGameActive) return;
        var delta = inputValue.Get<Vector2>();

        _broomXPos += delta.x * broomConfig.BroomSpeed;
        _broomXPos = Math.Clamp(_broomXPos, broomConfig.MinBroomXPos * transform.localScale.x,
            broomConfig.MaxBroomXPos * transform.localScale.x);
            
        broomModel.localPosition = new Vector3(_broomXPos, 0.25f, 2.5f);
        
        Debug.Log("Delta X is: " + delta.x);
        if (delta.x != 0)
        { 
            _sweepingTimeInSeconds += Time.deltaTime;
            _sweepingColor.color = Color.Lerp(_sweepingColor.color, _endingSweepingColor, Time.deltaTime * broomConfig.LerpSpeed);
            if(_sweepingTimeInSeconds >= 0.5 && _sweepingTimeInSeconds <= 1)
            {
                EndSweepingMinigame();
                _sweepingTimeInSeconds = 0;
            }
        }
    }
    
    public override string InteractionText(IInteractor interactor) => string.Empty;
}