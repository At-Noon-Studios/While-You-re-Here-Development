using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class ChopController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerHoldPoint; // assign your HeldObjectController's hold transform here
        [SerializeField] private Stump stump;
        [SerializeField] private Animator axeAnimator;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip impactSound;
        [SerializeField] private AudioClip swingSound;
        [SerializeField] private AudioClip gruntSound;
        [SerializeField] private AudioClip chopSound;

        [Header("Swipe")]
        [SerializeField] private float swipeThreshold = 100f;

        private Vector2 _swipeStart;
        private bool _swipeReady;

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _swipeStart = Mouse.current.position.ReadValue();
                _swipeReady = true;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && _swipeReady)
            {
                var swipeEnd = Mouse.current.position.ReadValue();
                var verticalSwipe = swipeEnd.y - _swipeStart.y;

                if (verticalSwipe < -swipeThreshold && IsHoldingAxe() && stump.HasLog)
                {
                    ChopLog();
                }

                _swipeReady = false;
            }
        }

        private bool IsHoldingAxe()
        {
            if (playerHoldPoint == null) return false;
            // Detect axe by presence under the hold point
            return playerHoldPoint.GetComponentInChildren<axe.AxePickable>() != null;
        }

        private void ChopLog()
        {
            if (audioSource != null)
            {
                if (swingSound != null) audioSource.PlayOneShot(swingSound);
                if (gruntSound != null) audioSource.PlayOneShot(gruntSound);
            }

            if (axeAnimator != null)
                axeAnimator.SetTrigger("Chop");

            if (audioSource != null && chopSound != null)
                audioSource.PlayOneShot(chopSound);

            var pickableLog = stump.GetLog();
            if (pickableLog == null) return;

            var logBehaviour = pickableLog.GetComponent<LogBehaviour>();
            if (logBehaviour == null) return;

            logBehaviour.GetLogHit(); // log tracks hits and splits itself when threshold met

            if (audioSource != null && impactSound != null)
                audioSource.PlayOneShot(impactSound);
        }
    }
}