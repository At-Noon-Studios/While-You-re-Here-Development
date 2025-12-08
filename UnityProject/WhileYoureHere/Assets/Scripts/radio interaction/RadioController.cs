using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using player_controls;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = System.Diagnostics.Debug;

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
       [HideInInspector] public RadioStateMachine radioStateMachine;

        [SerializeField] private Canvas OnCanvas;
        [SerializeField] private Canvas OfTuneCanvas;
        [SerializeField] private Canvas SlideCanvas;

        private void Start()
        {

            audioSource = GetComponent<AudioSource>();
            movementController = player.GetComponent<MovementController>();
            cameraController = player.GetComponentInChildren<CameraController>();
            tuneValue = 1f/radioTracks.Length;
            radioStateMachine =  new RadioStateMachine();
            radioStateMachine.ChangeState(new RadioOffState(this));
            isTuning = false;
        }
        

        private void Update()
        {
            radioStateMachine.CurrentState.Update();
            // if (isTuning)
            // {
            //     PositionCamera();
            // }
            // else camera.position = player.position+transform.up*3;
            //
            // HandleMouseMovement();
            // if (_radioOn && !_tuningLock) TuneRadio(tuneValue);
        }

        public float getTuningTimer() => radioData.tuningWaitTime;
        public void PositionCamera()
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

        public void HandleMouseMovement()
        {
            print("HandleMouseMovement being called");

            var current = Mouse.current.position.ReadValue();
            var deltaX = current.x - lastMousePos.x;
            tuneValue = Mathf.Clamp01(tuneValue + deltaX * radioData.sensitivity);
            MoveSlider(deltaX);
            lastMousePos = current;
            print("deltaX= " + deltaX );
            

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

        private IEnumerator TuningStateRoutine()
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
            OnCanvas.gameObject.SetActive(false);
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
            OnCanvas.gameObject.SetActive(true);
        }
        

        public void OnPowerPressed()
        {
            switch  (radioStateMachine.CurrentState)
            {
               case RadioOffState:
                   radioStateMachine.ChangeState(new RadioOnState(this));
                   break;
               case RadioOnState:
                   radioStateMachine.ChangeState(new RadioOffState(this));
                   break;
            }
        }
        public void OnTunePressed()
        {
            switch (radioStateMachine.CurrentState)
            {
                case RadioOnState:
                    radioStateMachine.ChangeState(new TuningState(this));
                    break;
                case TuningState:
                    radioStateMachine.ChangeState(new RadioOnState(this));
                    break;
            } 
        }

        public void SlideCanvasStatus(bool value)
        {
            SlideCanvas.gameObject.SetActive(value);
        }

        public void ShowOffCanvas()
        {
            OfTuneCanvas.gameObject.SetActive(true);
            OnCanvas.gameObject.SetActive(false);
            SlideCanvas.gameObject.SetActive(false);
        }

        public void ShowOnCanvas()
        {
            OnCanvas.gameObject.SetActive(true);
            OfTuneCanvas.gameObject.SetActive(false);
            SlideCanvas.gameObject.SetActive(false);
        }
        public void TurnRadioOn()
        {
            PlayStatic();
        }

        public void PlayStatic ()=> PlayClip(radioTracks[0].audioClip);

        public void TurnRadioOff()
        {
            audioSource.Stop();
            ExitTuningMode();
        }

        public void TuneRadio()
        {
            print("tuning radio being called");
            var stationSpacing = 1f / radioTracks.Length;
            var newIndex = Mathf.FloorToInt(tuneValue / stationSpacing);
            newIndex = Mathf.Clamp(newIndex, 0, radioTracks.Length - 1);
            if (currentRadioIndex == newIndex && audioSource.clip == radioTracks[newIndex].audioClip) return;
            currentRadioIndex = newIndex;
            PlayClip(radioTracks[newIndex].audioClip);
            if (newIndex == 2) StartCoroutine(TuningStateRoutine());
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSource.clip == clip && audioSource.isPlaying) return;
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }

        public bool OnCorrectChannel() => radioTracks[currentRadioIndex].audioClip == radioTracks[2].audioClip;
       
    }
}