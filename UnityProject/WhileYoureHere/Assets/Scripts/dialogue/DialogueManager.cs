using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    private Dictionary<string, DialogueNode> dialogueNodes = new Dictionary<string, DialogueNode>();
    private DialogueNode currentNode;

    public void StartDialogue(List<DialogueNode> nodes, string startingNodeID)
    {
        dialogueNodes.Clear();
        foreach (var node in nodes)
        {
            if (!dialogueNodes.ContainsKey(node.nodeID))
                dialogueNodes[node.nodeID] = node;
        }

        DisplayNode(startingNodeID);
    }

    private void DisplayNode(string nodeID)
    {
        if (!dialogueNodes.ContainsKey(nodeID))
        {
            Debug.LogError($"Dialogue node with ID '{nodeID}' not found!");
            return;
        }

        currentNode = dialogueNodes[nodeID];

        speakerText.text = currentNode.speakerName;
        dialogueText.text = currentNode.dialogueText;

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        foreach (var choice in currentNode.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            var tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = choice.choiceText;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                DisplayNode(choice.targetNodeID);
            });
        }
    }
}