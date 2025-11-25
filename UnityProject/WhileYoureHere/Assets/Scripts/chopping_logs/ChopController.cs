using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class ChopController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerHoldPoint;  // where the axe is held
        [SerializeField] private Stump stump;
        [SerializeField] private Transform axe;              // the axe object

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private AudioClip placeOnStumpSound;
        [SerializeField] private AudioClip woodChopSound;
        [SerializeField] private AudioClip crackSound;
        [SerializeField] private AudioClip axeImpactSound;
        [SerializeField] private AudioClip axeSwingSound;
        [SerializeField] private AudioClip gruntSound;

        [Header("Swipe/Stroke Settings")]
        [SerializeField] private float swipeThreshold = 80f;

        private Vector2 _swipeStart;
        private bool _strokeReady;        // player pulled mouse upward
        private bool _strokeDescending;   // player swinging downward

        private void Update()
        {
            HandleAxeStroke();
        }

        private void HandleAxeStroke()
        {
            if (!IsHoldingAxe()) return;

            // Start stroke
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _swipeStart = Mouse.current.position.ReadValue();
                _strokeReady = true;
                _strokeDescending = false;
            }

            if (Mouse.current.leftButton.isPressed && _strokeReady)
            {
                Vector2 currentPos = Mouse.current.position.ReadValue();
                float deltaY = currentPos.y - _swipeStart.y;

                // Phase 1: Pull axe upward
                if (deltaY > swipeThreshold)
                {
                    _strokeDescending = true;
                }

                // Phase 2: Swing downward (must follow-up after lifting)
                if (_strokeDescending && deltaY < -swipeThreshold)
                {
                    _strokeReady = false;
                    _strokeDescending = false;

                    PerformAxeSwing();
                }
            }
        }

        private bool IsHoldingAxe()
        {
            var axeBehaviour = playerHoldPoint.GetComponentInChildren<AxeBehaviour>();
            return axeBehaviour != null && axeBehaviour.IsHeld;
        }


        private void PerformAxeSwing()
        {
            if (!stump.HasLog) return;

            // Play swing sounds
            if (axeSwingSound) audioSource.PlayOneShot(axeSwingSound);
            if (gruntSound) audioSource.PlayOneShot(gruntSound);

            // Check if the axe is above the log (correct hit)
            if (!IsAxeAboveLog())
            {
                // Wrong hit: dull impact
                if (axeImpactSound) audioSource.PlayOneShot(axeImpactSound);
                Debug.Log("Incorrect hit: axe not above log.");
                return;
            }

            // Correct chop sound
            if (woodChopSound) audioSource.PlayOneShot(woodChopSound);

            // Damage the log
            var pickableLog = stump.GetLog();
            if (pickableLog == null) return;

            var logBehaviour = pickableLog.GetComponent<LogBehaviour>();
            if (logBehaviour == null) return;

            logBehaviour.GetLogHit();
        }

        private bool IsAxeAboveLog()
        {
            if (!stump.HasLog) return false;

            Transform log = stump.GetLog().transform;
            return axe.position.y > log.position.y + 0.1f;
        }
    }
}
