using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace chopping_logs
{
    public class ChopUIManager : MonoBehaviour
    {
        public static ChopUIManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private Image mouseUpImage;
        [SerializeField] private Image mouseDownImage;
        [SerializeField] private Image guideLineImage;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning("Duplicate ChopUIManager destroyed.");
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("ChopUIManager Awake: initialized and hiding all UI.");
            HideAllUI();
        }

        public void ShowUI()
        {
            Debug.Log("ChopUIManager: Showing UI.");
            if (mouseUpImage != null) mouseUpImage.enabled = true;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = true;
        }

        public void HideAllUI()
        {
            Debug.Log("ChopUIManager: Hiding all UI.");
            if (mouseUpImage != null) mouseUpImage.enabled = false;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = false;
        }

        public void OnLook(InputValue value)
        {
            if (!Stump.CurrentMinigameActive)
            {
                Debug.Log("ChopUIManager: Mouse input ignored, minigame not active.");
                return;
            }

            var delta = value.Get<Vector2>();
            var yDelta = delta.y;
            Debug.Log($"ChopUIManager: OnLook called, Y delta = {yDelta}");

            if (guideLineImage != null && !guideLineImage.enabled)
            {
                guideLineImage.enabled = true;
                Debug.Log("ChopUIManager: Guideline image enabled.");
            }

            if (yDelta > 5.0f)
            {
                if (mouseUpImage != null) mouseUpImage.enabled = false;
                if (mouseDownImage != null) mouseDownImage.enabled = true;
                Debug.Log("ChopUIManager: Mouse moved UP → showing MouseDown icon.");
            }
            else if (yDelta < -5.0f)
            {
                if (mouseUpImage != null) mouseUpImage.enabled = true;
                if (mouseDownImage != null) mouseDownImage.enabled = false;
                Debug.Log("ChopUIManager: Mouse moved DOWN → showing MouseUp icon.");
            }
            else
            {
                Debug.Log("ChopUIManager: Mouse movement minimal → no icon change.");
            }
        }
    }
}
