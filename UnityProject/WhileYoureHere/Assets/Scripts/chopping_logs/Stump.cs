using UnityEngine;
using UnityEngine.InputSystem;
using picking_up_objects;

namespace chopping_logs
{
    public class Stump : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerHoldPoint;       // player's hold slot
        [SerializeField] private Transform logPlacementPoint;     // where placed logs snap

        [Header("Placement Settings")]
        [SerializeField] private float snapRadius = 0.4f;         // how close log must be to snap

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip placeSound;

        private Pickable _currentLog;

        private void Update()
        {
            HandlePlacement();
        }

        /// <summary>
        /// Handles pressing E to place log onto stump.
        /// </summary>
        private void HandlePlacement()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame)
                return;

            // Stump already has a log
            if (_currentLog != null)
                return;

            if (playerHoldPoint == null)
            {
                Debug.LogWarning("Stump: playerHoldPoint not assigned.");
                return;
            }

            // Check if player is actually holding something
            var heldPickable = playerHoldPoint.GetComponentInChildren<Pickable>();
            if (heldPickable == null)
                return;

            // Distance check for snapping
            float distToStump = Vector3.Distance(
                heldPickable.transform.position,
                logPlacementPoint.position
            );

            if (distToStump > snapRadius)
            {
                Debug.Log("Log too far from stump, dropping.");
                heldPickable.Drop();
                return;
            }

            // VALID placement → snap into position
            PlaceLogOnStump(heldPickable);
        }

        /// <summary>
        /// Performs the snapping & stabilization.
        /// </summary>
        private void PlaceLogOnStump(Pickable log)
        {
            Transform t = log.transform;

            // Snap to designated point
            t.position = logPlacementPoint.position;
            t.rotation = logPlacementPoint.rotation;

            // Tell the pickable it's placed (detaches from hand)
            log.Place();

            // Freeze physics to keep log steady
            Rigidbody rb = log.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            // Notify the log behaviour
            var logBehaviour = log.GetComponent<LogBehaviour>();
            if (logBehaviour != null)
                logBehaviour.PlaceOnStump(this);

            _currentLog = log;

            // Play wood placement sound
            if (audioSource != null && placeSound != null)
                audioSource.PlayOneShot(placeSound);

            Debug.Log("Log placed & snapped on stump.");
        }

        /// <summary>
        /// Clears the stump reference when log splits/destroys.
        /// </summary>
        public void ClearLog()
        {
            _currentLog = null;
        }

        public Pickable GetLog() => _currentLog;

        public bool HasLog => _currentLog != null;

        private void OnDrawGizmosSelected()
        {
            // Visualize snap radius
            if (logPlacementPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(logPlacementPoint.position, snapRadius);
            }
        }
    }
}
