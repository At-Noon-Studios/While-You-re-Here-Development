using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.dialogue;
using UnityEngine;
using ScriptableObjects.Dialogue;

namespace dialogue
{
    public class DialogueLoader : MonoBehaviour
    {
        [SerializeField] private DialogueManager dialogueManager;
        private DialogueInteractionConfig _config;
        [SerializeField] private GameObject dialogueUI;
        public void StartDialogue(DialogueInteractionConfig config)
        {
            if (config.dialogueNodes.Count == 0 || dialogueManager == null) return;
            _config = config;
            dialogueUI.SetActive(true); 
            StartCoroutine(StartDialogueNextFrame());
        }

        private IEnumerator StartDialogueNextFrame()
        {
            yield return null;
            dialogueManager.StartDialogue(_config);
        }
    }
}