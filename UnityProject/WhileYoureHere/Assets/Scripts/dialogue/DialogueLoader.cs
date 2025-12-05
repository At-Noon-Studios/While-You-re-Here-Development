using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects.Dialogue;

namespace dialogue
{
    public class DialogueLoader : MonoBehaviour
    {
        [SerializeField] private List<DialogueNode> dialogueNodes;
        [SerializeField] private string startingNodeId = "start";
        [SerializeField] private DialogueManager dialogueManager;

        public void StartDialogue(List<DialogueNode> configDialogueNodes)
        {
            if (dialogueNodes.Count == 0 || dialogueManager == null) return;
            StartCoroutine(StartDialogueNextFrame());
        }

        private IEnumerator StartDialogueNextFrame()
        {
            yield return null;
            dialogueManager.StartDialogue(dialogueNodes, startingNodeId);
        }
    }
}