using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private TextMeshProUGUI interactPromptObjectName;

        private void Awake()
        {
            Instance = this;
        }

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
