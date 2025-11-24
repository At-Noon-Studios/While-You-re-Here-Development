using UnityEngine;
using UnityEngine.InputSystem;
using picking_up_objects;

namespace chopping_logs
{
    public class Stump : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerHoldPoint;   // assign the player's hold transform here
        [SerializeField] private Transform logPlacementPoint; // where the log sits on the stump
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip placeSound;

        private Pickable _currentLog;

        private void Update()
        {
            HandlePlacement();
        }

        /// <summary>
        /// Press E to place the held log onto the stump.
        /// </summary>
        private void HandlePlacement()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame)
                return;

            if (_currentLog != null)
                return; // stump already occupied

            // Find a Pickable currently under the player's hold point
            if (playerHoldPoint == null)
            {
                Debug.LogWarning("Stump: playerHoldPoint is not assigned.");
                return;
            }

            var heldPickable = playerHoldPoint.GetComponentInChildren<Pickable>();
            if (heldPickable == null)
                return; // nothing is being held

            // Release from being held using Pickable's own API
            heldPickable.Place();

            // Snap log to stump placement point
            var t = heldPickable.transform;
            t.position = logPlacementPoint != null ? logPlacementPoint.position : transform.position;
            t.rotation = logPlacementPoint != null ? logPlacementPoint.rotation : transform.rotation;

            // Optional: ensure it sits stable
            var rb = heldPickable.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                rb.useGravity = true;
            }

            _currentLog = heldPickable;

            // Play placement sound
            if (audioSource != null && placeSound != null)
                audioSource.PlayOneShot(placeSound);

            Debug.Log("Log placed on stump");
        }

        /// <summary>
        /// Clears the stump (after chopping).
        /// </summary>
        public void ClearLog()
        {
            _currentLog = null;
        }

        /// <summary>
        /// Returns the log currently on the stump.
        /// </summary>
        public Pickable GetLog()
        {
            return _currentLog;
        }

        /// <summary>
        /// Checks if stump currently has a log.
        /// </summary>
        public bool HasLog => _currentLog != null;
    }
}