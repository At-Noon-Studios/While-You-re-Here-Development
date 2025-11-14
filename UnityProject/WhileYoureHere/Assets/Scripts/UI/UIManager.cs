using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private TextMeshProUGUI interactPromptObjectName;
    
        public void ShowInteractPrompt(string objectName)
        {
            interactPromptObjectName.text = objectName;
            interactPrompt.SetActive(true);
        }

        public void HideInteractPrompt()
        {
            interactPrompt.SetActive(false);
        }
    }
}
