using System;
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
        public static DialogueManager Instance;
        
        [Header("UI")] 
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private GameObject dialogueImage;
        [SerializeField] private GameObject navigationObject;
        
        [Header("Timing")] [SerializeField] private float letterDelay = 0.05f;
        [SerializeField] private float sentenceDelay = 1.5f;
        
        public event Action OnLastSentenceFinished;

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
        private float _currentResumeAudioTime;
        private bool _isTyping;
        private bool _cameraStopped;
        private bool _movementStopped;
        private DialogueInteractionConfig _currentInteractionConfig;
        
        private List<Button> _choiceButtons = new List<Button>();
        private List<Image> _choiceImages = new List<Image>();
        private int _currentSelectedChoice = 0;

        private bool IsChoiceNavigationActive => _choiceButtons.Count > 0 && _choiceImages.Count > 0;

        [SerializeField] private float volume = 1;
        private int _resumeCharIndex;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            _ui = UIManager.Instance;
        }

        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _movement = player.GetComponent<MovementController>();
                _cameraController =
                    player.GetComponentInChildren<CameraController>();
                _playerInput = player.GetComponent<PlayerInput>();

                if (_playerInput != null)
                {
                    _playerInput.actions["SkipDialogue"].performed += OnSkipDialogue;
                    _playerInput.actions["Navigate"].performed += OnNavigate;
                    _playerInput.actions["Confirm"].performed += OnConfirm;
                }
            }
        }
        
        private void OnDestroy()
        {
            if (_playerInput != null)
            {
                _playerInput.actions["SkipDialogue"].performed -= OnSkipDialogue;
                _playerInput.actions["Navigate"].performed -= OnNavigate;
                _playerInput.actions["Confirm"].performed -= OnConfirm;
            }
        }
        
        private void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!gameObject.activeSelf || _choiceButtons.Count == 0) return;
            
            float value = ctx.ReadValue<float>();
            
            if (_currentSelectedChoice >= 0 && _currentSelectedChoice < _choiceButtons.Count)
            {
                var currentImg = _choiceImages[_currentSelectedChoice];
                if (currentImg != null && _currentNode.choices[_currentSelectedChoice].normalSprite != null)
                    currentImg.sprite = _currentNode.choices[_currentSelectedChoice].normalSprite;
            }
            
            if (value > 0)
            {
                _currentSelectedChoice = (_currentSelectedChoice + 1) % _choiceButtons.Count;
            }
            else if (value < 0)
            {
                _currentSelectedChoice--;
                if (_currentSelectedChoice < 0)
                    _currentSelectedChoice = _choiceButtons.Count - 1;
            }
            
            
            if (_currentSelectedChoice >= 0 && _currentSelectedChoice < _choiceButtons.Count)
            {
                var newImg = _choiceImages[_currentSelectedChoice];
                if (newImg != null && _currentNode.choices[_currentSelectedChoice].selectedSprite != null)
                    newImg.sprite = _currentNode.choices[_currentSelectedChoice].selectedSprite;
            }
        }
        
        private void OnConfirm(InputAction.CallbackContext ctx)
        {
            if (!gameObject.activeSelf || _choiceButtons.Count == 0) return;
            if (_currentSelectedChoice < 0 || _currentSelectedChoice >= _choiceButtons.Count) return;
            
            _choiceButtons[_currentSelectedChoice].onClick.Invoke();
        }

        private void OnSkipDialogue(InputAction.CallbackContext ctx)
        {
            if (!gameObject.activeSelf || _currentNode == null ||
                choicesContainer.childCount > 0) return;
            var nrSentences = _currentNode.sentences.Count - 1;
            if (_isTyping)
            {
                _isTyping = false;
                _ui.ShowDialogue(_currentNode.speakerName, _currentFullSentence,
                    _currentNode.speakerNameColor);
            }
            else
            {
                    ProceedToNextSentence();
            }
        }

        public void StartDialogue(DialogueInteractionConfig interactionConfig)
        {
            EventSystem.current?.SetSelectedGameObject(null);

            _currentInteractionConfig = interactionConfig;

            _nodes.Clear();
            foreach (var n in interactionConfig.dialogueNodes)
                _nodes[n.nodeID] = n;

            gameObject.SetActive(true);
            _movementStopped = interactionConfig.pausePlayerMovement;
            _cameraStopped = interactionConfig.pauseCameraMovement;

            if (!interactionConfig.pauseOnlyDuringChoices)
            {
                if (_movementStopped && _movement != null)
                    _movement.PauseMovement();
                
                if (_cameraStopped && _cameraController != null)
                    _cameraController.PauseCameraMovement();
            }

            if (interactionConfig.showCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            DisplayNode(interactionConfig.dialogueNodes[0].nodeID);
        }


        public void StartRadioDialogue(DialogueNode node, float resumeTime = 0,
            int startSentenceIndex = 0)
        {
            if (node == null) return;

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

            _sentenceRoutine = StartCoroutine(
                TypeSentenceWithResume(_activeSentences[_sentenceIndex],
                    resumeTime));
        }

        private void DisplayNode(string id)
        {
            if (!_nodes.TryGetValue(id, out _currentNode))
            {
                EndDialogue();
                return;
            }

            _choiceButtons.Clear();
            _choiceImages.Clear();
            _currentSelectedChoice = 0;

            foreach (Transform child in choicesContainer)
                Destroy(child.gameObject);

            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);

            bool hasSentences = _currentNode.sentences != null && _currentNode.sentences.Count > 0;

            if (dialogueImage != null)
                dialogueImage.SetActive(hasSentences);
            
            if (_currentNode.choices == null || _currentNode.choices.Count == 0)
            {
                if (navigationObject != null)
                    navigationObject.SetActive(false);
            }
            else
            {
                if (navigationObject != null)
                    navigationObject.SetActive(true);
            }
            
            if (hasSentences)
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
            
            if (_currentInteractionConfig != null && _currentInteractionConfig.pauseOnlyDuringChoices)
            {
                if (_movementStopped && _movement != null)
                    _movement.ResumeMovement();
                
                if (_cameraStopped && _cameraController != null)
                    _cameraController.ResumeCameraMovement();
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

        private IEnumerator TypeSentenceWithResume(DialogueSentence sentence,
            float resumeTime)
        {
            _isTyping = false;
            _currentFullSentence = sentence.text;
            _ui.ShowDialogue(_currentNode.speakerName, sentence.text,
                _currentNode.speakerNameColor);

            if (sentence.audio is null) yield break;
            if (_audioSource is null)
                _audioSource = GameObject.FindWithTag(sentence.tagOfAudioSource)
                    .GetComponent<AudioSource>();
            if (_currentNode.nodeID == "radio_static")
            {
                PlayStaticAudio(sentence.audio);
                yield break;
            }

            /*
             * this is needed if we decided to keep rollen text instead of viewing it at once
             */
            resumeTime = PlayResumedAudio(sentence, resumeTime);
            // _resumeCharIndex = Mathf.FloorToInt((resumeTime / sentence.audio.length) * sentence.text.Length);
            // _resumeCharIndex = Mathf.Clamp(_resumeCharIndex, 0, sentence.text.Length - 1);
            //
            // string output = sentence.text.Substring(0, _resumeCharIndex);
            // _ui.ShowDialogue(_currentNode.speakerName, output, _currentNode.speakerNameColor);

            // This can be disabled for now the text can be displayed at once and still can be skipped
            // for (int i = _resumeCharIndex; i < sentence.text.Length; i++)
            // {
            //     if (!_isTyping)
            //     {
            //         _ui.ShowDialogue(_currentNode.speakerName, sentence.text);
            //     }
            //     else
            //     {
            //         output += sentence.text[i];
            //         _ui.ShowDialogue(_currentNode.speakerName, output);
            //
            //         yield return new WaitForSeconds(letterDelay);
            //     }
            // }

            resumeTime = _audioSource.time;
            _currentResumeAudioTime = resumeTime;
            //* same here

            // if (resumeTime == 0f)
            // {
            //     _currentResumeAudioTime = _audioSource.time;
            //     resumeTime = sentence.audio.length - 0.1f;
            // }

            yield return new WaitForSeconds(sentence.audio.length - resumeTime);

            ProceedToNextSentence();
        }

        private void ProceedToNextSentence()
        {
            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);

            if (_audioSource != null)
                _audioSource.Stop();

            if (_activeSentences == null)
            {
                EndDialogue();
                return;
            }

            if (_sentenceIndex < _activeSentences.Length)
                _sentenceIndex += 1;
            
            if (_activeSentences == null ||
                _sentenceIndex >= _activeSentences.Length)
            {
                if (_currentNode.flag != null)
                {
                    _currentNode.flag.currentValue = true;
                }
                
                EndDialogue();
                OnLastSentenceFinished?.Invoke();
                _sentenceIndex = 0;
                return;
            }

            _currentResumeAudioTime = 0f;
            _sentenceRoutine = StartCoroutine(
                TypeSentenceWithResume(_activeSentences[_sentenceIndex], 0f));
        }

        private float PlayResumedAudio(DialogueSentence sentence,
            float resumeTime)
        {
            var startedClip = sentence.audio;
            _audioSource.clip = sentence.audio;
            _audioSource.volume = _currentInteractionConfig != null 
                ? _currentInteractionConfig.dialogueVolume : 1f;
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
            _audioSource.volume = _currentInteractionConfig != null 
                ? _currentInteractionConfig.dialogueVolume : 1f;
            _audioSource.Play();
        }

        private IEnumerator TypeSentence(DialogueSentence sentence)
        {
            _isTyping = true;
            _currentFullSentence = sentence.text;

            if (sentence.audio != null)
            {
                if (_audioSource == null ||
                    _audioSource.gameObject.CompareTag(sentence.tagOfAudioSource) == false)
                {
                    var goWithTag = !string.IsNullOrEmpty(sentence.tagOfAudioSource)
                        ? GameObject.FindWithTag(sentence.tagOfAudioSource)
                        : null;

                    if (goWithTag != null)
                    {
                        _audioSource = goWithTag.GetComponent<AudioSource>();
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"[DialogueManager] No GameObject with tag '{sentence.tagOfAudioSource}' found for dialogue audio.");
                        _audioSource = null;
                    }
                }

                if (_audioSource != null)
                {
                    _audioSource.Stop();
                    _audioSource.volume = _currentInteractionConfig != null 
                        ? _currentInteractionConfig.dialogueVolume : 1f;
                    _audioSource.clip = sentence.audio;
                    _audioSource.loop = false;
                    _audioSource.Play();
                }
            }

            string output = "";
            foreach (char c in sentence.text)
            {
                if (!_isTyping)
                {
                    _ui.ShowDialogue(_currentNode.speakerName, sentence.text,
                        _currentNode.speakerNameColor);
                    yield break;
                }

                output += c;
                _ui.ShowDialogue(_currentNode.speakerName, output,
                    _currentNode.speakerNameColor);
                yield return new WaitForSeconds(letterDelay);
            }

            _isTyping = false;

            if (sentence.audio != null && _audioSource != null)
            {
                while (_audioSource != null && _audioSource.isPlaying)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(sentenceDelay);
            }
            PlayNextSentence();
        }

        private void CreateChoices()
        {
            _choiceButtons.Clear();
            _choiceImages.Clear();
            _currentSelectedChoice = 0;
            
            if (_currentInteractionConfig != null && _currentInteractionConfig.pauseOnlyDuringChoices)
            {
                if (_movementStopped && _movement != null)
                    _movement.PauseMovement();
                
                if (_cameraStopped && _cameraController != null)
                    _cameraController.PauseCameraMovement();
            }
            
            int index = 0;
            foreach (var choice in _currentNode.choices)
            {
                var btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
                var btn = btnObj.GetComponent<Button>();
                var img = btnObj.GetComponent<Image>() ?? btnObj.GetComponentInChildren<Image>();

                _choiceButtons.Add(btn);
                _choiceImages.Add(img);
                
                int capturedIndex = index;
                
                if (img != null && choice.normalSprite != null)
                    img.sprite = choice.normalSprite;

                btn.onClick.AddListener(() =>
                {
                    DisplayNode(choice.targetNodeID);
                });

                if (img != null)
                {
                    var trigger = btnObj.GetComponent<UnityEngine.EventSystems.EventTrigger>() 
                                  ?? btnObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();

                    void SetSprite(Sprite s) { img.sprite = s; }

                    var enter = new UnityEngine.EventSystems.EventTrigger.Entry
                    {
                        eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
                    };
                    enter.callback.AddListener(_ => 
                    { 
                        if (choice.selectedSprite != null) 
                        {
                            if (_currentSelectedChoice >= 0 && _currentSelectedChoice < _choiceImages.Count)
                            {
                                var oldImg = _choiceImages[_currentSelectedChoice];
                                if (oldImg != null && _currentNode.choices[_currentSelectedChoice].normalSprite != null)
                                    oldImg.sprite = _currentNode.choices[_currentSelectedChoice].normalSprite;
                            }
                            _currentSelectedChoice = capturedIndex;
                            SetSprite(choice.selectedSprite);
                        }
                    });
                    trigger.triggers.Add(enter);

                    var exit = new UnityEngine.EventSystems.EventTrigger.Entry
                    {
                        eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit
                    };
                    exit.callback.AddListener(_ => 
                    { 
                        if (capturedIndex != _currentSelectedChoice && choice.normalSprite != null) 
                            SetSprite(choice.normalSprite); 
                    });
                    trigger.triggers.Add(exit);
                }
                
                index++;
            }

            if (_choiceButtons.Count > 0 && _choiceImages[0] != null && 
                _currentNode.choices[0].selectedSprite != null)
            {
                _choiceImages[0].sprite = _currentNode.choices[0].selectedSprite;
            }
            
            if (_currentInteractionConfig != null && _currentInteractionConfig.showCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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
            if (_sentenceRoutine != null)
                StopCoroutine(_sentenceRoutine);
            _ui?.HideDialogue();
            gameObject.SetActive(false);
            _audioSource?.Stop();
            
            if (_movementStopped && _movement != null)
                _movement.ResumeMovement();
            
            if (_cameraStopped && _cameraController != null)
                _cameraController.ResumeCameraMovement();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public int GetCurrentSentenceIndex() => _sentenceIndex;
        public DialogueNode CurrentNode { get; set; }

        public float GetCurrentResumeAudioTime()
        {
            if (_audioSource == null || !_audioSource.isPlaying)
                return 0f;
            _currentResumeAudioTime = _audioSource.time;
            return _currentResumeAudioTime;
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
        
    }
}