using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using player_controls;
using PlayerControls;
using ScriptableObjects.dialogue;
using ScriptableObjects.Dialogue;
using UI;

namespace dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Timing")]
        [SerializeField] private float letterDelay = 0.05f;
        [SerializeField] private float sentenceDelay = 1.5f;

        private AudioSource _audioSource;
        private UIManager _ui;
        private MovementController _movement;
        private CameraController _cameraController;
        private PlayerInput _playerInput;

        private readonly Dictionary<string, DialogueNode> _nodes = new();
        private DialogueNode _currentNode;
        private DialogueSentence[] _activeSentences;
        private int _sentenceIndex;
        private string _currentFullSentence;
        private Coroutine _sentenceRoutine;
        private bool _isTyping;

        private bool _cameraStopped;
        private bool _movementStopped;

        [SerializeField] private float volume = 1;

        private void Start()
        {
            _ui = UIManager.Instance;

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _movement = player.GetComponent<MovementController>();
                _cameraController = player.GetComponentInChildren<CameraController>();
                _playerInput = player.GetComponent<PlayerInput>();

                if (_playerInput != null)
                    _playerInput.actions["SkipDialogue"].performed += OnSkipDialogue;
            }
        }

        private void OnSkipDialogue(InputAction.CallbackContext ctx)
        {
            if (!gameObject.activeSelf || choicesContainer.childCount > 0) return;

            if (_isTyping)
            {
                _isTyping = false;
                _ui.ShowDialogue(_currentNode.speakerName, _currentFullSentence);
            }
            else
            {
                PlayNextSentence();
            }
        }

        public void StartDialogue(DialogueInteractionConfig interactionConfig)
        {
            EventSystem.current?.SetSelectedGameObject(null);

            _nodes.Clear();
            foreach (var n in interactionConfig.dialogueNodes) _nodes[n.nodeID] = n;

            gameObject.SetActive(true);
            _movementStopped = interactionConfig.pausePlayerMovement;
            _cameraStopped = interactionConfig.pauseCameraMovement;
            DisplayNode(interactionConfig.dialogueNodes[0].nodeID);
        }

        private void DisplayNode(string id)
        {
            if (!_nodes.TryGetValue(id, out _currentNode))
            {
                EndDialogue();
                return;
            }

            foreach (Transform child in choicesContainer)
                Destroy(child.gameObject);

            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);

            if (_currentNode.sentences?.Count > 0)
            {
                _activeSentences = _currentNode.sentences.ToArray();
                _sentenceIndex = 0;
                PlayNextSentence();
            }
            else if (_currentNode.choices?.Count > 0)
            {
                CreateChoices();
            }
            else
            {
                HandleNextNodeOrEnd();
            }
        }

        private void PlayNextSentence()
        {
            if (_currentNode.flag != null)
            {
                _currentNode.flag.currentValue = true;
            }
            if (_sentenceIndex >= _activeSentences.Length)
            {
                if (_currentNode.choices?.Count > 0)
                    CreateChoices();
                else
                    HandleNextNodeOrEnd();
                return;
            }

            DialogueSentence sentence = _activeSentences[_sentenceIndex++];
            _sentenceRoutine = StartCoroutine(TypeSentence(sentence));
        }

        private IEnumerator TypeSentence(DialogueSentence sentence)
        {
            _isTyping = true;
            _currentFullSentence = sentence.text;

            if (sentence.audio != null)
            {
                if (_audioSource == null) _audioSource = GameObject.FindWithTag(sentence.tagOfAudioSource).GetComponent<AudioSource>();
                _audioSource.Stop();
                _audioSource = GameObject.FindWithTag(sentence.tagOfAudioSource).GetComponent<AudioSource>();
                _audioSource.volume = volume;
                _audioSource.clip = sentence.audio;
                _audioSource.Play();
            }

            string output = "";
            foreach (char c in sentence.text)
            {
                if (!_isTyping)
                {
                    _ui.ShowDialogue(_currentNode.speakerName, sentence.text);
                    yield break;
                }

                output += c;
                _ui.ShowDialogue(_currentNode.speakerName, output);
                yield return new WaitForSeconds(letterDelay);
            }

            _isTyping = false;
            yield return new WaitForSeconds(sentenceDelay);
            
            PlayNextSentence();
        }

        private void CreateChoices()
        {
            foreach (var choice in _currentNode.choices)
            {
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = choice.choiceText;
                btn.GetComponent<Button>().onClick.AddListener(() => DisplayNode(choice.targetNodeID));
            }

            EventSystem.current?.SetSelectedGameObject(null);
        }

        private void HandleNextNodeOrEnd()
        {
            if (!string.IsNullOrEmpty(_currentNode.targetNodeID))
                DisplayNode(_currentNode.targetNodeID);
            else
                EndDialogue();
        }

        private void EndDialogue()
        {
            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);

            _ui.HideDialogue();
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _audioSource.Stop();
            if (_movementStopped) _movement?.ResumeMovement();
            if (_cameraStopped) _cameraController?.ResumeCameraMovement();
        }
    }
}
