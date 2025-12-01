using player_controls;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace radio_interaction
{
    public class RadioController : MonoBehaviour
    {
        [SerializeField] private RadioTracks[] radioTracks;
        [SerializeField] private AudioClip staticClip;
        [SerializeField] private Transform player;
        [SerializeField] private EventChannel clickTune;

        private AudioSource audioSource;
        private int currentRadioIndex = -1;
        private float tuneThreshold = 0.1f;
        public bool radioOn;
        private bool previousRadioState;
        private float tuneValue;
        private bool isTuning;
        private Vector2 lastMousePos;
        public float sensitivity = 0.002f;
        private bool leftMouseButtonPressed;
        MovementController movementController;
        CameraController cameraController;


        public void OnEnable()
        {
            clickTune.OnClick += mouseDown;
        }
    
        public void OnDisable()
        {
            clickTune.OnClick -= mouseDown;

        }

        public void mouseDown(bool isPressed)
        {
            leftMouseButtonPressed=isPressed;
        }
    
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            movementController = player.GetComponent<MovementController>();
            cameraController = player.GetComponentInChildren<CameraController>();
            isTuning = false;
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

        public bool tuning()=>isTuning;
   
        public void EnterTuningMode()
        {
            if (!radioOn)
            {
                return;
            }
        
            if (isTuning) return;
            isTuning = true;
            //pause screen
            movementController?.PauseMovement();
            cameraController?.PauseCameraMovement();
        
            lastMousePos = Mouse.current.position.ReadValue();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }

        public void ExitTuningMode()
        {
            isTuning = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        
            //unpause screen
            movementController?.ResumeMovement();
            cameraController?.ResumeCameraMovement();
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
}