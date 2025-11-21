using player_controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class RadioController : MonoBehaviour
{
    [SerializeField] private RadioTracks[] radioTracks;
    [SerializeField] private AudioClip staticClip;
    [SerializeField] private Transform startButtonLocation;
    [SerializeField] private Transform player;
    private AudioSource audioSource;
    private int currentRadioIndex = -1;
    private float tuneThreshold = 0.1f;
    public bool radioOn;
    private bool previousRadioState;
    private bool isDragging;
    private float tuneValue;
    private Vector3 startMousePos;
    private bool isTuning;
    private Vector2 lastMousePos;
    public float sensitivity = 0.002f;
    private bool leftMouseButtonPressed;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnClick(InputValue inputValue)
    {
        leftMouseButtonPressed = inputValue.isPressed;
        
    }
    private void Update()
    {
        // Handle tuning mode input
        if (isTuning)
        {
            if (leftMouseButtonPressed)
            {
                Vector2 current = Mouse.current.position.ReadValue();
                float deltaX = current.x - lastMousePos.x;

                tuneValue = Mathf.Clamp01(tuneValue + deltaX * sensitivity);

                lastMousePos = current;
            }

            if (!leftMouseButtonPressed)
            {
                ExitTuningMode();
            }
        }

        // Perform radio tuning while on
        if (radioOn)
        {
            TuneRadio(tuneValue);
        }

        // Detect radio turning off
        if (!radioOn && previousRadioState)
        {
            Debug.Log("Radio turned OFF — stopping audio.");
            audioSource.Stop();
            currentRadioIndex = -1;
        }

        previousRadioState = radioOn;
    }

    public void EnterTuningMode()
    {
        if (!radioOn)
        {
            Debug.Log("Radio is off, cannot tune.");
            return;
        }

        var movementController = player.GetComponent<MovementController>();
        movementController?.PauseMovement();

        var cameraController = player.GetComponentInChildren<CameraController>();
        cameraController?.PauseCameraMovement();
        isTuning = true;
        lastMousePos = Mouse.current.position.ReadValue();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Entered tuning mode.");
    }

    public void ExitTuningMode()
    {
        isTuning = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Exited tuning mode.");
    }
   
    public void TurnRadioOn()
    {
        radioOn = true;
    }

    public void TurnRadioOff()
    {
        radioOn = false;
    }
    private void TuneRadio(float value)
    {
         // value = tuneSlider.value;
        var stationSpacing = 1f / radioTracks.Length;

        var newIndex = Mathf.FloorToInt(value / stationSpacing);
        var stationCenter = newIndex * stationSpacing;

        var isTuned = Mathf.Abs(value - stationCenter) < tuneThreshold;

        switch (isTuned)
        {
            case true when newIndex != currentRadioIndex:
                currentRadioIndex = newIndex;
                Debug.Log("Tuned to station: " + newIndex);
                PlayClip(radioTracks[newIndex].audioClip);
                break;
            case false when audioSource.clip != staticClip:
                currentRadioIndex = -1;
                Debug.Log("Static");
                PlayClip(staticClip);
                break;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}