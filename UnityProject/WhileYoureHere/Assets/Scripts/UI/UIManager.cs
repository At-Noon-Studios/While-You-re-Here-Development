using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [Header("Interact Prompt")]
        [SerializeField] private InteractionPrompt interactPrompt;

        [Header("Dialogue UI")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject dialoguePanel;

        [CanBeNull] private Coroutine _currentPulseRoutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void ShowInteractPrompt(string text, bool allowed)
        {
            interactPrompt.ShowInteractPrompt(text, allowed);
        }

        public void HideInteractPrompt()
        {
            interactPrompt.HideInteractPrompt();
        }

        public void PulseInteractPrompt()
        {
            interactPrompt.PulseInteractPrompt();
        }

    
        public void ShowDialogue(string speaker, string line)
        {
            if (dialoguePanel) dialoguePanel.SetActive(true);
            if (!dialogueText) return;
            dialogueText.text = !string.IsNullOrEmpty(speaker)
                    ? $"<b>{speaker}</b>\n{line}"
                    : line;
        }

        public void HideDialogue()
        {
            dialoguePanel?.SetActive(false);
        }
    }
}