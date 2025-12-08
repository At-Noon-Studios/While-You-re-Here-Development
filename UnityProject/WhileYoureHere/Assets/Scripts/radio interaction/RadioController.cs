using System;
using System.Collections;
using System.Collections.Generic;
using player_controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace radio_interaction
{
    public class RadioController : MonoBehaviour
    {
        [Header("audio tracks")]
        [SerializeField] private RadioTracks[] radioTracks;
        [Header("radio data")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform slider;
        [SerializeField] private RadioData radioData;
        [SerializeField] private Transform camera;
        private AudioSource audioSource;
        private int currentRadioIndex;
        private bool _radioOn;
        private bool _tuningLock;
        private float tuneValue;
        private Vector2 lastMousePos;
        private bool isTuning;
        private MovementController movementController;
        private CameraController cameraController;

        [SerializeField] private Canvas OnCanvas;
        [SerializeField] private Canvas OfTuneCanvas;
        [SerializeField] private Canvas SlideCanvas;

        private void Start()
        {

            audioSource = GetComponent<AudioSource>();
            movementController = player.GetComponent<MovementController>();
            cameraController = player.GetComponentInChildren<CameraController>();
            tuneValue = 1f/radioTracks.Length;
            isTuning = false;
        }
        

        private void Update()
        {
            if (isTuning)
            {
                PositionCamera();
            }
            else camera.position = player.position+transform.up*3;
            
            HandleMouseMovement();
            if (_radioOn && !_tuningLock) TuneRadio(tuneValue);
        }

        private void PositionCamera()
        {
            var position = Vector3.Lerp(
                camera.transform.position,
                transform.position + -transform.right*2,
                Time.deltaTime * 1f
            );
            camera.position=position;
                
            var targetRotation = Quaternion.LookRotation(
                transform.position - camera.transform.position,
                Vector3.up
            );
                
            camera.transform.rotation = Quaternion.Lerp(
                camera.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }

        private void HandleMouseMovement()
        {
            if (!isTuning || _tuningLock) return;
            var current = Mouse.current.position.ReadValue();
            var deltaX = current.x - lastMousePos.x;
            tuneValue = Mathf.Clamp01(tuneValue + deltaX * radioData.sensitivity);
            MoveSlider(deltaX);
            lastMousePos = current;
            

        }
        private void MoveSlider(float deltaX)
        {
            var move = deltaX *radioData.SliderSensitivity();
            var pos = slider.localPosition;
            pos.z -= move;
            pos.z = Mathf.Clamp(pos.z, radioData.MinSliderPos(), radioData.MaxSliderPos());
            slider.localPosition = pos;
        }
        public bool Tuning()=>isTuning;
        public bool RadioOnStatus() => _radioOn;

        private IEnumerator TuningState()
        {
            
            if (_tuningLock) yield break;
            var holdTimer = 0f;
            while (holdTimer < radioData.tuningWaitTime)
            {
                if (audioSource.isPlaying && audioSource.clip == radioTracks[2].audioClip)
                {
                    holdTimer += Time.deltaTime;  
                }
                else
                {
                    holdTimer = 0f;                
                }
                yield return null;
            }
            _tuningLock = true;
            isTuning = false;
            yield return new WaitForSeconds(radioData.tuningWaitTime);
            ExitTuningMode();
            _tuningLock = false;
        }
        
        public void EnterTuningMode()
        {
            
            if (!_radioOn) return;
            if (isTuning) return;
            
            SlideCanvas.gameObject.SetActive(true);
            OfTuneCanvas.gameObject.SetActive(false);
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
            movementController?.ResumeMovement();
            cameraController?.ResumeCameraMovement();
            SlideCanvas.gameObject.SetActive(false);
            OfTuneCanvas.gameObject.SetActive(true);
        }
   
        public void TurnRadioOn()
        {
            var staticClip = radioTracks[0].audioClip;
            _radioOn = true;
            OfTuneCanvas.gameObject.SetActive(true);
            PlayClip(staticClip);
        }

        public void TurnRadioOff()
        {
            _radioOn = false;
            audioSource.Stop();
            OfTuneCanvas.gameObject.SetActive(false);
            OnCanvas.gameObject.SetActive(true);
            ExitTuningMode();
        }
        
        private void TuneRadio(float value)
        {
            var stationSpacing = 1f / radioTracks.Length;
            var newIndex = Mathf.FloorToInt(value / stationSpacing);
            newIndex = Mathf.Clamp(newIndex, 0, radioTracks.Length - 1);
            if (currentRadioIndex == newIndex && audioSource.clip == radioTracks[newIndex].audioClip) return;
            currentRadioIndex = newIndex;
            PlayClip(radioTracks[newIndex].audioClip);
            if (newIndex == 2) StartCoroutine(TuningState());
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