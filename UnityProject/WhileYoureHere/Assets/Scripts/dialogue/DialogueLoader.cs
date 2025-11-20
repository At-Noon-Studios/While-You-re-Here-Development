using System.Collections.Generic;
using ScriptableObjects.Dialogue;
using UnityEngine;

namespace dialogue
{
    public class DialogueLoader : MonoBehaviour
    {
        [Header("Dialogue Configuration")]
        [SerializeField] private List<DialogueNode> dialogueNodes;
        [SerializeField] private string startingNodeId = "start";

        [Header("Dialogue References")]
        [SerializeField] private DialogueManager dialogueManager;

        private void Awake()
        {
            if (dialogueManager == null)
            {
                Debug.LogError("DialogueManager not found in the scene!");
            }
        }

        public void StartDialogue()
        {
            if (dialogueManager != null && dialogueNodes.Count > 0)
            {
                dialogueManager.StartDialogue(dialogueNodes, startingNodeId);
            }
        }
    }
}