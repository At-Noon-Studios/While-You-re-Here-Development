using System.Collections; 
using System.Collections.Generic;
using Dialogue;
using ScriptableObjects.Dialogue;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI;

namespace dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;
        
        [Header("Controller References")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private CameraController cameraController;
        
        [Header("Interaction References")]
        [SerializeField] private InteractPrompt interactPrompt;

        private const float WaitingTime = 2f;
        private UIManager _uiManager;

        private readonly Dictionary<string, DialogueNode> _dialogueNodes = new Dictionary<string, DialogueNode>();
        private DialogueNode _currentNode;

        private void Start()
        {
            _uiManager = UIManager.Instance;
        }

        public void StartDialogue(List<DialogueNode> nodes, string startingNodeID)
        {
            EventSystem.current?.SetSelectedGameObject(null);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _dialogueNodes.Clear();
            foreach (var n in nodes) _dialogueNodes[n.nodeID] = n;

            gameObject.SetActive(true);
            DisplayNode(startingNodeID);
        }

        private void DisplayNode(string nodeID)
        {
            if (!_dialogueNodes.TryGetValue(nodeID, out var node))
            {
                EndDialogue();
                return;
            }

            _currentNode = node;

            _uiManager?.ShowDialogue(_currentNode.speakerName, _currentNode.dialogueText);

            foreach (Transform child in choicesContainer)
                Destroy(child.gameObject);

            bool hasText = !string.IsNullOrWhiteSpace(_currentNode.dialogueText);
            bool hasChoices = _currentNode.choices is { Count: > 0 };

            if (!hasText && !hasChoices)
            {
                EndDialogue();
                return;
            }

            if (hasText && !hasChoices)
            {
                StartCoroutine(AutoContinue());
                return;
            }

            foreach (var choice in _currentNode.choices)
            {
                var button = Instantiate(choiceButtonPrefab, choicesContainer);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = choice.choiceText;
                button.GetComponent<Button>().onClick.AddListener(() => DisplayNode(choice.targetNodeID));
            }

            EventSystem.current?.SetSelectedGameObject(null);
        }

        private IEnumerator AutoContinue()
        {
            yield return new WaitForSeconds(WaitingTime);
            EndDialogue();
        }
        
        private void EndDialogue()
        {
            _uiManager?.HideDialogue();

            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            movementController?.ResumeMovement();
            cameraController?.ResumeCameraMovement();
            interactPrompt?.EndInteraction();
        }
    }
}