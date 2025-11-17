using System;
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

        private DialogueManager _dialogueManager;

        [Obsolete("Obsolete")]
        private void Awake()
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();

            if (_dialogueManager == null)
            {
                Debug.LogError("DialogueManager not found in the scene!");
            }
        }

        [Obsolete("Obsolete")]
        public void StartDialogue()
        {
            if (_dialogueManager != null && dialogueNodes.Count > 0)
            {
                _dialogueManager.StartDialogue(dialogueNodes, startingNodeId);
            }
        }
    }
}