using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    private Dictionary<string, DialogueNode> dialogueNodes = new Dictionary<string, DialogueNode>();
    private DialogueNode currentNode;

    public void StartDialogue(List<DialogueNode> nodes, string startingNodeID)
    {
        EventSystem.current?.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        dialogueNodes.Clear();
        foreach (var n in nodes) dialogueNodes[n.nodeID] = n;
        gameObject.SetActive(true);
        DisplayNode(startingNodeID);
    }

    private void DisplayNode(string nodeID)
    {
        if (!dialogueNodes.ContainsKey(nodeID))
        {
            EndDialogue();
            return;
        }

        currentNode = dialogueNodes[nodeID];
        speakerText.text = currentNode.speakerName;
        dialogueText.text = currentNode.dialogueText;

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        bool hasText = !string.IsNullOrWhiteSpace(currentNode.dialogueText);
        bool hasChoices = currentNode.choices != null && currentNode.choices.Count > 0;

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

        foreach (var choice in currentNode.choices)
        {
            var button = Instantiate(choiceButtonPrefab, choicesContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            button.GetComponent<Button>().onClick.AddListener(() => DisplayNode(choice.targetNodeID));
        }

        EventSystem.current?.SetSelectedGameObject(null);
    }

    private IEnumerator AutoContinue()
    {
        yield return new WaitForSeconds(2f);
        EndDialogue();
    }

    public void EndDialogue()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        FindObjectOfType<MovementController>()?.ResumeMovement();
        FindObjectOfType<CameraController>()?.ResumeCameraMovement();
        FindObjectOfType<InteractPrompt>()?.EndInteraction();
    }
}