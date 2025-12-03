using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace chopping_logs
{
    public class ChopUIManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Stump stumpReference;

        [SerializeField] private Image mouseUpImage;
        [SerializeField] private Image mouseDownImage;
        [SerializeField] private Image guideLineImage;

        private void Start()
        {
            if (mouseUpImage != null) mouseUpImage.enabled = false;
            if (mouseDownImage != null) mouseDownImage.enabled = false;
            if (guideLineImage != null) guideLineImage.enabled = false;

            Debug.Log("ChopUIManager initialized. All UI hidden.");
        }

        public void OnLook(InputValue value)
        {
            if (stumpReference == null || !stumpReference.MinigameActive)
            {
                Debug.Log("Mouse input ignored: minigame not active.");
                return;
            }

            // Show guideline when minigame is active
            if (guideLineImage != null && !guideLineImage.enabled)
                guideLineImage.enabled = true;

            // DEFAULT STATE (when minigame first starts)
            // Ensure UP icon is always shown at the start
            if (!mouseUpImage.enabled && !mouseDownImage.enabled)
            {
                mouseUpImage.enabled = true;
                mouseDownImage.enabled = false;
                Debug.Log("Default state: Showing UP icon.");
            }

            var delta = value.Get<Vector2>();
            float yDelta = delta.y;

            Debug.Log($"Mouse Y Delta: {yDelta}");

            // Player moves UP → tell them to move DOWN next
            if (yDelta > 1f)
            {
                mouseUpImage.enabled = false;
                mouseDownImage.enabled = true;
                Debug.Log("Mouse moved up → showing DOWN icon.");
            }
            // Player moves DOWN → tell them to move UP next
            else if (yDelta < -1f)
            {
                mouseUpImage.enabled = true;
                mouseDownImage.enabled = false;
                Debug.Log("Mouse moved down → showing UP icon.");
            }
            else
            {
                Debug.Log("Mouse Y movement minimal → no icon change.");
            }
        }
    }
}