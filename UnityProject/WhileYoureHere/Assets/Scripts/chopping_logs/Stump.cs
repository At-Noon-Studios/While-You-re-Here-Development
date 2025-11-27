using System;
using Interactable;
using picking_up_objects;
using player_controls;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [Header("Log placement settings")]
        [SerializeField] private Transform logPlaceholder;

        private GameObject _logObject;
        private bool _hasLog;

        public bool HasLog => _hasLog;
        public GameObject LogObject => _logObject;

        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
            if (_hasLog) return; // stump already occupied

            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<HeldObjectController>();
            var heldObject = heldController?.GetHeldObject();

            if (heldObject is Pickable pickableLog && pickableLog.CompareTag("Log"))
            {
                // Snap to placement point
                pickableLog.Place();
                pickableLog.transform.position = logPlaceholder.position;
                pickableLog.transform.rotation = logPlaceholder.rotation;

                // Freeze physics
                var rb = pickableLog.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                // Parent to stump
                pickableLog.transform.SetParent(logPlaceholder);

                // Track log
                _logObject = pickableLog.gameObject;
                _hasLog = true;

                // Disable Pickable so log can't be picked up again
                pickableLog.enabled = false;

                Debug.Log("Log Placed");
                heldController.ClearHeldObject();
            }
        }

        public void ClearLog()
        {
            if (_logObject)
            {
                _logObject.transform.SetParent(null);

                var rb = _logObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }

                // Re‑enable Pickable if log is freed
                var pickable = _logObject.GetComponent<Pickable>();
                if (pickable) pickable.enabled = true;
            }

            _logObject = null;
            _hasLog = false;
        }

        public void StartMinigame()
        {
            Debug.Log("Mini-game started for chopping the log!");

            // Freeze camera rotation
            var cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController)
            {
                cameraController.PauseCameraMovement();
            }

            // Freeze player movement
            var playerMovement = GameObject.FindWithTag("Player")?.GetComponent<MovementController>();
            if (playerMovement)
            {
                playerMovement.PauseMovement();
            }
        }

        protected override string InteractionText()
        {
            if (_hasLog) return "Stump (Occupied)";

            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<HeldObjectController>();
            var heldObject = heldController?.GetHeldObject();

            if (heldObject is Pickable pickableLog && pickableLog.CompareTag("Log"))
            {
                return "Place Log on Stump (E)";
            }

            return "Stump (Empty)";
        }

        public override void OnHoverEnter() => base.OnHoverEnter();
        public override void OnHoverExit() => base.OnHoverExit();
    }
}