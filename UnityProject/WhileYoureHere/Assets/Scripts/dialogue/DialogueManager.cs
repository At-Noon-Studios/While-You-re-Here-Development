using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using player_controls;
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

        private AudioSource audioSource;
        private UIManager ui;
        private MovementController movement;
        private CameraController cameraController;
        private PlayerInput playerInput;

        private readonly Dictionary<string, DialogueNode> nodes = new();
        private DialogueNode currentNode;
        private DialogueSentence[] activeSentences;
        private int sentenceIndex;
        private string currentFullSentence;
        private Coroutine sentenceRoutine;
        private bool isTyping;

        private void Start()
        {
            ui = UIManager.Instance;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                movement = player.GetComponent<MovementController>();
                cameraController = player.GetComponentInChildren<CameraController>();
                playerInput = player.GetComponent<PlayerInput>();
            }

            if (playerInput != null)
                playerInput.actions["SkipDialogue"].performed += OnSkipDialogue;
        }

        private void OnSkipDialogue(InputAction.CallbackContext ctx)
        {
            if (!gameObject.activeSelf || choicesContainer.childCount > 0)
                return;

            if (isTyping)
            {
                isTyping = false;
                ui.ShowDialogue(currentNode.speakerName, currentFullSentence);
            }
            else
            {
                PlayNextSentence();
            }
        }

        public void StartDialogue(List<DialogueNode> dialogueNodes, string startId)
        {
            EventSystem.current?.SetSelectedGameObject(null);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            nodes.Clear();
            foreach (var n in dialogueNodes) nodes.TryAdd(n.nodeID, n);

            gameObject.SetActive(true);
            DisplayNode(startId);
        }

        private void DisplayNode(string id)
        {
            if (!nodes.TryGetValue(id, out currentNode))
            {
                EndDialogue();
                return;
            }

            foreach (Transform child in choicesContainer)
                Destroy(child.gameObject);

            bool hasSentences = currentNode.sentences is { Count: > 0 };
            bool hasChoices = currentNode.choices is { Count: > 0 };

            if (sentenceRoutine != null)
                StopCoroutine(sentenceRoutine);

            if (hasSentences)
            {
                activeSentences = currentNode.sentences.ToArray();
                sentenceIndex = 0;
                PlayNextSentence();
            }
            else if (hasChoices)
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
            if (sentenceIndex >= activeSentences.Length)
            {
                bool hasChoices = currentNode.choices is { Count: > 0 };
                if (hasChoices)
                    CreateChoices();
                else
                    HandleNextNodeOrEnd();
                return;
            }

            DialogueSentence sentence = activeSentences[sentenceIndex++];
            if (sentenceRoutine != null)
                StopCoroutine(sentenceRoutine);

            sentenceRoutine = StartCoroutine(TypeSentence(sentence));
        }

        private IEnumerator TypeSentence(DialogueSentence sentence)
        {
            isTyping = true;
            currentFullSentence = sentence.text;

            if (sentence.audio != null)
            {
                audioSource.Stop();
                audioSource.clip = sentence.audio;
                audioSource.Play();
            }

            string output = "";
            foreach (char c in sentence.text)
            {
                if (!isTyping)
                {
                    ui.ShowDialogue(currentNode.speakerName, sentence.text);
                    yield break;
                }

                output += c;
                ui.ShowDialogue(currentNode.speakerName, output);
                yield return new WaitForSeconds(letterDelay);
            }

            isTyping = false;
            yield return new WaitForSeconds(sentenceDelay);

            PlayNextSentence();
        }

        private void CreateChoices()
        {
            foreach (var choice in currentNode.choices)
            {
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = choice.choiceText;
                btn.GetComponent<Button>().onClick.AddListener(() => DisplayNode(choice.targetNodeID));
            }

            EventSystem.current?.SetSelectedGameObject(null);
        }

        private void HandleNextNodeOrEnd()
        {
            if (!string.IsNullOrEmpty(currentNode.targetNodeID))
                DisplayNode(currentNode.targetNodeID);
            else
                EndDialogue();
        }

        private void EndDialogue()
        {
            if (sentenceRoutine != null)
                StopCoroutine(sentenceRoutine);

            ui.HideDialogue();
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            audioSource.Stop();
            movement?.ResumeMovement();
            cameraController?.ResumeCameraMovement();
        }
    }
}
