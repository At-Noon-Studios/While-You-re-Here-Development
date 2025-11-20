using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [Header("Interact Prompt")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private TextMeshProUGUI interactPromptObjectName;

        [Header("Dialogue UI")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject dialoguePanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        public void ShowInteractPrompt(string objectName)
        {
            if (interactPromptObjectName != null)
                interactPromptObjectName.text = objectName;

            interactPrompt?.SetActive(true);
        }

        public void HideInteractPrompt()
        {
            interactPrompt?.SetActive(false);
        }

        public void ShowDialogue(string speaker, string line)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);

            if (dialogueText != null)
            {
                dialogueText.text = !string.IsNullOrEmpty(speaker)
                    ? $"<b>{speaker}</b>\n{line}"
                    : line;
            }
        }

        public void HideDialogue()
        {
            dialoguePanel?.SetActive(false);
        }
    }
}