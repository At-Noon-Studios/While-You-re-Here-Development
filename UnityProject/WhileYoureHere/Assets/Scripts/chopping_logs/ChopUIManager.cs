using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using gamestate;

namespace chopping_logs
{
    public class ChopUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image mouseUpImage;
        [SerializeField] private Image mouseDownImage;
        [SerializeField] private Image guideLineImage;

        private bool _minigameActive = false;

        private void Awake()
        {
            HideAllUI();
        }

        private void OnEnable()
        {
            var gsm = GamestateManager.GetInstance();
            gsm.OnChopMinigameStarted += OnMinigameStarted;
            gsm.OnChopMinigameEnded += OnMinigameEnded;
        }

        private void OnDisable()
        {
            var gsm = GamestateManager.GetInstance();
            gsm.OnChopMinigameStarted -= OnMinigameStarted;
            gsm.OnChopMinigameEnded -= OnMinigameEnded;
        }

        private void OnMinigameStarted()
        {
            _minigameActive = true;
            ResetUI();
            Debug.Log("Minigame started → UI activated.");
        }

        private void OnMinigameEnded()
        {
            _minigameActive = false;
            HideAllUI();
            Debug.Log("Minigame ended → UI hidden.");
        }

        private void ResetUI()
        {
            if (mouseUpImage != null) mouseUpImage.enabled = true;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = true;
        }

        private void HideAllUI()
        {
            if (mouseUpImage != null) mouseUpImage.enabled = false;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = false;
        }

        public void OnLook(InputValue value)
        {
            if (!_minigameActive) return;

            var delta = value.Get<Vector2>();
            var yDelta = delta.y;

            if (guideLineImage != null && !guideLineImage.enabled)
                guideLineImage.enabled = true;

            if (yDelta > 1f)
            {
                if (mouseUpImage != null) mouseUpImage.enabled = false;
                if (mouseDownImage != null) mouseDownImage.enabled = true;
                Debug.Log("Mouse moved UP → showing MouseDown icon.");
            }
            else if (yDelta < -1f)
            {
                if (mouseUpImage != null) mouseUpImage.enabled = true;
                if (mouseDownImage != null) mouseDownImage.enabled = false;
                Debug.Log("Mouse moved DOWN → showing MouseUp icon.");
            }
        }
    }
}