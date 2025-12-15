using System.Collections.Generic;
using dialogue;
using player_controls;
using ScriptableObjects.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

namespace radio_interaction
{
    public class RadioController : MonoBehaviour
    {
        [Header("audio tracks")] [SerializeField]
        private RadioTracks[] radioTracks;

        [Header("radio data")] [HideInInspector]
        public RadioStateMachine RadioStateMachine;
        [SerializeField] private Transform player;
        [SerializeField] private Transform slider;
        [SerializeField] private RadioData radioData;
        [SerializeField] private Transform cam;
        [SerializeField] private Canvas onStateCanvas;
        [SerializeField] private Canvas offStateCanvas;
        [SerializeField] private Canvas slideCanvas;

        private const string ClearChannelNodeName = "radio_clear_node";
        private const string StaticChannelNodeName = "radio_static";

        private int _currentStationIndex;
        private float _tuningNormalized;
        private float _stationSpacing;

        private Vector2 _lastMousePos;
        private MovementController _movementController;
        private CameraController _cameraController;
        private Vector3 _currentCameraPosition;
        private Quaternion _currentCameraRotation;
        private DialogueNode _lastPlayedNode;

        private readonly struct RadioChannelProgress
        {
            public readonly int SentenceIndex;
            public readonly float AudioTime;

            public RadioChannelProgress(int sentenceIndex, float audioTime)
            {
                SentenceIndex = sentenceIndex;
                AudioTime = audioTime;
            }
        }

        private readonly Dictionary<DialogueNode, RadioChannelProgress> _nodeSentenceProgress = new();

        [SerializeField] private DialogueManager dialogueManager;

        private void Start()
        {
            _movementController = player.GetComponent<MovementController>();
            _cameraController = player.GetComponentInChildren<CameraController>();

            _stationSpacing = (radioTracks != null && radioTracks.Length > 0) ? 1f / radioTracks.Length : 0f;
            _tuningNormalized = _stationSpacing;

            RadioStateMachine = new RadioStateMachine();
            RadioStateMachine.ChangeState(new RadioOffState(this));

            _currentStationIndex = 0;
        }

        private void Update()
        {
            RadioStateMachine.Update();
        }

        #region public methods

        public float GetTuningTimer() => radioData.tuningWaitTime;

        public void PositionTuningCamera()
        {
            var position = Vector3.Lerp(
                cam.transform.position,
                transform.position + -transform.right * 2,
                Time.deltaTime * 5f
            );
            cam.position = position;

            var targetRotation = Quaternion.LookRotation(
                transform.position - cam.transform.position,
                Vector3.up
            );

            cam.transform.rotation = Quaternion.Lerp(
                cam.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }

        public bool ResetCamera()
        {
            const float moveSpeed = 5f;
            const float rotateSpeed = 5f;

            cam.position = Vector3.Lerp(
                cam.position,
                _currentCameraPosition,
                Time.deltaTime * moveSpeed
            );
            cam.rotation = Quaternion.Lerp(
                cam.rotation,
                _currentCameraRotation,
                Time.deltaTime * rotateSpeed
            );

            var positionDone = Vector3.Distance(cam.position, _currentCameraPosition) < 0.01f;
            var rotationDone = Quaternion.Angle(cam.rotation, _currentCameraRotation) < 0.5f;
            return positionDone && rotationDone;
        }

        public void HandleMouseMovement()
        {
            var current = Mouse.current.position.ReadValue();
            var deltaX = current.x - _lastMousePos.x;

            _tuningNormalized = Mathf.Clamp01(_tuningNormalized + deltaX * radioData.sensitivity);

            MoveSlider(deltaX);
            _lastMousePos = current;
        }

        public void EnterTuningMode()
        {
            _cameraController?.PauseCameraMovement();
            _currentCameraPosition = cam.transform.position;
            _currentCameraRotation = cam.transform.rotation;

            onStateCanvas.gameObject.SetActive(false);
            _movementController?.PauseMovement();

            _lastMousePos = Mouse.current.position.ReadValue();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void ExitTuningMode()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _movementController?.ResumeMovement();
            _cameraController?.ResumeCameraMovement();

            onStateCanvas.gameObject.SetActive(true);
        }

        public void OnPowerPressed()
        {
            switch (RadioStateMachine.CurrentState)
            {
                case RadioOffState:
                    RadioStateMachine.ChangeState(new RadioOnState(this));
                    break;
                case RadioOnState:
                case TuningState:
                    RadioStateMachine.ChangeState(new RadioOffState(this));
                    break;
            }
        }

        public void OnTunePressed()
        {
            switch (RadioStateMachine.CurrentState)
            {
                case RadioOnState:
                    RadioStateMachine.ChangeState(new TuningState(this));
                    break;
                case TuningState:
                    RadioStateMachine.ChangeState(new RadioOnState(this));
                    break;
            }
        }

        public void SlideCanvasStatus(bool value)
        {
            slideCanvas.gameObject.SetActive(value);
        }

        public void ShowOffCanvas()
        {
            offStateCanvas.gameObject.SetActive(true);
            onStateCanvas.gameObject.SetActive(false);
            slideCanvas.gameObject.SetActive(false);
        }

        public void ShowOnCanvas()
        {
            onStateCanvas.gameObject.SetActive(true);
            offStateCanvas.gameObject.SetActive(false);
            slideCanvas.gameObject.SetActive(false);
        }

        public void TurnRadioOn()
        {
            if (!TryValidateRadioSetup(out var error))
            {
                Debug.LogError(error, this);
                return;
            }

            if (IsStaticChannel(_currentStationIndex))
            {
                var staticNode = radioTracks[_currentStationIndex].dialogueNode;
                if (staticNode == null)
                {
                    Debug.LogWarning("TurnRadioOn: Static channel has no dialogueNode assigned.", this);
                    return;
                }

                dialogueManager.StartRadioDialogue(staticNode, 0f);
                _lastPlayedNode = staticNode;
                return;
            }

            var nodeToPlay = _lastPlayedNode != null ? _lastPlayedNode : radioTracks[_currentStationIndex].dialogueNode;
            if (nodeToPlay == null)
            {
                Debug.LogWarning("TurnRadioOn: No dialogue node available to play.", this);
                return;
            }

            var resumeIndex = 0;
            var resumeTime = 0f;
            if (_nodeSentenceProgress.TryGetValue(nodeToPlay, out var saved))
            {
                resumeIndex = saved.SentenceIndex;
                resumeTime = saved.AudioTime;
            }

            dialogueManager.StartRadioDialogue(nodeToPlay, resumeTime, resumeIndex);
            _lastPlayedNode = nodeToPlay;
        }

        public void TurnRadioOff()
        {
            // Save progress for the node that was actually playing (if any)
            var nodeToSave = dialogueManager != null ? dialogueManager.CurrentNode : null;
            if (nodeToSave == null && radioTracks is { Length: > 0 } &&
                _currentStationIndex >= 0 && _currentStationIndex < radioTracks.Length)
            {
                nodeToSave = radioTracks[_currentStationIndex].dialogueNode;
            }

            if (nodeToSave != null)
            {
                SaveCurrentDialogueProgress(nodeToSave);
                _lastPlayedNode = nodeToSave;
            }

            dialogueManager.EndDialogue();
        }

        public void TuneRadio()
        {
            if (radioTracks == null || radioTracks.Length == 0)
                return;

            var spacing = _stationSpacing > 0f ? _stationSpacing : (1f / radioTracks.Length);

            var newIndex = Mathf.FloorToInt(_tuningNormalized / spacing);
            newIndex = Mathf.Clamp(newIndex, 0, radioTracks.Length - 1);

            if (_currentStationIndex == newIndex)
                return;

            OnStationChanged(newIndex);
        }

        public bool OnCorrectChannel() => radioTracks[_currentStationIndex].nodeName == ClearChannelNodeName;

        public bool DonePlayingCorrectChannel()
        {
            if (dialogueManager == null || _lastPlayedNode == null || _lastPlayedNode.sentences == null)
                return false;

            // maybe adding a condition for the last sentence of the node and then playing music if finished
            return dialogueManager.GetCurrentSentenceIndex() == _lastPlayedNode.sentences.Count;
        }

        #endregion

        #region private methods

        private bool TryValidateRadioSetup(out string error)
        {
            if (dialogueManager == null)
            {
                error = "TurnRadioOn: dialogueManager is not assigned.";
                return false;
            }

            if (radioTracks == null || radioTracks.Length == 0)
            {
                error = "TurnRadioOn: radioTracks is null or empty.";
                return false;
            }

            if (_currentStationIndex < 0 || _currentStationIndex >= radioTracks.Length)
            {
                error = $"TurnRadioOn: _currentStationIndex out of range: {_currentStationIndex}";
                return false;
            }

            error = null;
            return true;
        }

        private void MoveSlider(float deltaX)
        {
            var move = deltaX * radioData.SliderSensitivity();
            var pos = slider.localPosition;
            pos.z -= move;
            pos.z = Mathf.Clamp(pos.z, radioData.MinSliderPos(), radioData.MaxSliderPos());
            slider.localPosition = pos;
        }

        private bool IsStaticChannel(int index)
        {
            return radioTracks[index].nodeName == StaticChannelNodeName || radioTracks[index].dialogueNode == null;
        }

        private void SaveCurrentDialogueProgress(DialogueNode node)
        {
            if (node is null || dialogueManager == null) return;

            var progress = new RadioChannelProgress(
                dialogueManager.GetCurrentSentenceIndex(),
                dialogueManager.GetCurrentAudioTime()
            );

            _nodeSentenceProgress[node] = progress;
        }

        private void ChangeNode(DialogueNode newNode)
        {
            var oldNode = dialogueManager.CurrentNode;
            if (oldNode == newNode)
                return;

            if (oldNode != null)
                SaveCurrentDialogueProgress(oldNode);

            var resumeIndex = 0;
            var resumeTime = 0f;

            if (newNode != null && _nodeSentenceProgress.TryGetValue(newNode, out var saved))
            {
                resumeIndex = saved.SentenceIndex;
                resumeTime = saved.AudioTime;
            }

            Debug.Log($"Switching to {newNode.nodeID}, resumeIndex={resumeIndex}, resumeTime={resumeTime}");
            dialogueManager.StartRadioDialogue(newNode, resumeTime, resumeIndex);
            dialogueManager.CurrentNode = newNode;
        }

        private void OnStationChanged(int index)
        {
            _currentStationIndex = index;
            var node = radioTracks[index].dialogueNode;

            ChangeNode(node);
            _lastPlayedNode = node;
        }

        #endregion
    }
}