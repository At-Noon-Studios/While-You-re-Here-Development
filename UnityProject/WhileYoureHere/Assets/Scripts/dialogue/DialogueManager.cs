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
        [Header("UI")] [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Timing")] [SerializeField] private float letterDelay = 0.05f;
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
        private float _CurrentResumeAudioTime;
        private bool _isTyping;

        private bool _cameraStopped;
        private bool _movementStopped;

        [SerializeField] private float volume = 1;
        private int _resumeCharIndex;

        private void Awake()
        {
            _ui = UIManager.Instance;
        }

        private void Start()
        {
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
                // PlayNextSentence();
                ProceedToNextSentence();

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

        public void StartRadioDialogue(DialogueNode node, float resumeTime = 0, int startSentenceIndex = 0)
        {
            _nodes.Clear();
            _nodes[node.nodeID] = node;
            _currentNode = node;
            _activeSentences = node.sentences.ToArray();
            _sentenceIndex = startSentenceIndex;
            gameObject.SetActive(true);


            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);
            if (!_nodes.TryGetValue(node.nodeID, out _currentNode))
            {
                EndDialogue();
                return;
            }


            _sentenceRoutine = StartCoroutine(TypeSentenceWithResume(_activeSentences[_sentenceIndex], resumeTime));
            // PlayNextSentence();
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

        private IEnumerator TypeSentenceWithResume(DialogueSentence sentence, float resumeTime)
        {
            _isTyping = true;
            _currentFullSentence = sentence.text;
            _ui.ShowDialogue(_currentNode.speakerName, sentence.text);

            if (sentence.audio != null)
            {
                if (_audioSource == null)
                    _audioSource = GameObject.FindWithTag(sentence.tagOfAudioSource).GetComponent<AudioSource>();
                if (_currentNode.nodeID == "radio_static")
                {
                    PlayStaticAudio(sentence.audio);
                    yield break;
                }

                resumeTime = PlayResumedAudio(sentence, resumeTime);

                _resumeCharIndex = Mathf.FloorToInt((resumeTime / sentence.audio.length) * sentence.text.Length);
                _resumeCharIndex = Mathf.Clamp(_resumeCharIndex, 0, sentence.text.Length - 1);

                string output = sentence.text.Substring(0, _resumeCharIndex);
                _ui.ShowDialogue(_currentNode.speakerName, output);

                for (int i = _resumeCharIndex; i < sentence.text.Length; i++)
                {
                    if (!_isTyping)
                    {
                        _ui.ShowDialogue(_currentNode.speakerName, sentence.text);
                        yield break;
                    }

                    output += sentence.text[i];
                    _ui.ShowDialogue(_currentNode.speakerName, output);

                    yield return new WaitForSeconds(letterDelay);
                }

                resumeTime = _audioSource.time;
                _CurrentResumeAudioTime = _audioSource.time;
                if (resumeTime == 0)
                {
                    _CurrentResumeAudioTime = _audioSource.time;
                    resumeTime = sentence.audio.length - 0.5f;
                }
                
                // Debug.Log("resume time =" + resumeTime + "sentence length = " + sentence.audio.length);
                yield return new WaitForSeconds(sentence.audio.length - resumeTime);
                _isTyping = false;

                ProceedToNextSentence();
            }
        }

        private void ProceedToNextSentence()
        {
            if (_sentenceIndex < _activeSentences.Length)
                _sentenceIndex += 1;
            if (_activeSentences == null || _sentenceIndex >= _activeSentences.Length)
            {
                EndDialogue();
                _sentenceIndex = 0;
                return;
            }

            _sentenceRoutine = StartCoroutine(TypeSentenceWithResume(_activeSentences[_sentenceIndex], 0f));
        }

        private float PlayResumedAudio(DialogueSentence sentence, float resumeTime)
        {
            var startedClip = sentence.audio;
            _audioSource.clip = sentence.audio;
            _audioSource.volume = volume;
            resumeTime = Mathf.Clamp(resumeTime, 0f, startedClip.length);
            _audioSource.time = resumeTime;
            _audioSource.Play();
            _audioSource.loop = false;
            return resumeTime;
        }

        private void PlayStaticAudio(AudioClip clip)
        {
            _audioSource.loop = true;
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.Play();
        }

        private IEnumerator TypeSentence(DialogueSentence sentence)
        {
            _isTyping = true;
            _currentFullSentence = sentence.text;

            if (sentence.audio != null)
            {
                if (_audioSource == null)
                    _audioSource = GameObject.FindWithTag(sentence.tagOfAudioSource).GetComponent<AudioSource>();
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

        public void EndDialogue()
        {
            // need to ask Arian what to keep here otherwise i am gonna make a new method
            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);
            _ui?.HideDialogue();
            gameObject.SetActive(false);
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
            _audioSource?.Stop();
            // _movement?.ResumeMovement();
            // _cameraController?.ResumeCameraMovement();
        }

        public int GetCurrentSentenceIndex() => _sentenceIndex;
        public DialogueNode CurrentNode { get; set; }

        public float GetCurrentResumeAudioTime()
        {
            if (_audioSource == null || !_audioSource.isPlaying)
                return 0f;

            return _CurrentResumeAudioTime;
        }

        public float GetSentenceAudioTime(int sentenceIndex)
        {
            if (_currentNode == null || _currentNode.sentences == null)
                return 0f;

            if ((uint)sentenceIndex >= (uint)_currentNode.sentences.Count)
                return 0f;

            var sentence = _currentNode.sentences[sentenceIndex];
            var clip = sentence != null ? sentence.audio : null;
            return clip != null ? clip.length : 0f;
        }

        public bool SentenceRoutineStopped()
        {
            return _sentenceRoutine is null;
        }
    }
}