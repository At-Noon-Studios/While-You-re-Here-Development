using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    [Header("Dialogue Configuration")]
    public List<DialogueNode> dialogueNodes;
    public string startingNodeID = "start";

    private DialogueManager dialogueManager;

    void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found in the scene!");
        }
    }

    void Start()
    {
        if (dialogueManager != null && dialogueNodes.Count > 0)
        {
            dialogueManager.StartDialogue(dialogueNodes, startingNodeID);
        }
    }
}