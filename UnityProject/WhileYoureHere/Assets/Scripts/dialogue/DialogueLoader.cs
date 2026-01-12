using System.Collections;
using ScriptableObjects.dialogue;
using UnityEngine;

namespace dialogue
{
    public class DialogueLoader : MonoBehaviour
    {
        private DialogueManager _dialogueManager;
        private DialogueInteractionConfig _config;

        private void Start()
        {
            _dialogueManager = DialogueManager.Instance;
        }
        
        public void StartDialogue(DialogueInteractionConfig config)
        {
            if (config.dialogueNodes.Count == 0 || !_dialogueManager) return;
            _config = config;
            StartCoroutine(StartDialogueNextFrame());
        }

        private IEnumerator StartDialogueNextFrame()
        {
            yield return null;
            _dialogueManager.StartDialogue(_config);
        }
    }
}