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

        public bool HasLog => _hasLog;
        public bool MinigameActive { get; private set; }

        private GameObject _logObject;
        private bool _hasLog;

        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
            // If minigame has started, stump is no longer interactable
            if (MinigameActive) return;

            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<HeldObjectController>();
            var held = heldController?.GetHeldObject();

            // If NO LOG is placed → placing logic
            if (!_hasLog)
            {
                if (held is Pickable pickableLog && pickableLog.CompareTag("Log"))
                {
                    PlaceLog(pickableLog, heldController);
                }
                return;
            }

            // If LOG IS placed → start minigame only when holding axe
            if (held is Pickable axe && axe.CompareTag("Axe"))
            {
                StartMinigame();
            }
        }

        private void PlaceLog(Pickable pickableLog, HeldObjectController controller)
        {
            pickableLog.Place();
            pickableLog.transform.position = logPlaceholder.position;
            pickableLog.transform.rotation = logPlaceholder.rotation;

            var rb = pickableLog.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            pickableLog.transform.SetParent(logPlaceholder);
            pickableLog.enabled = false;

            _logObject = pickableLog.gameObject;
            _hasLog = true;

            controller.ClearHeldObject();

            Debug.Log("Log placed on stump.");
        }

        public void StartMinigame()
        {
            Debug.Log("Minigame started!");

            MinigameActive = true;

            var movement = GameObject.FindWithTag("Player")?.GetComponent<MovementController>();
            // if (movement)
                // movement.PauseMovement();
        }

        public void EndMinigame()
        {
            MinigameActive = false;

            var movement = GameObject.FindWithTag("Player")?.GetComponent<MovementController>();
            if (movement)
                movement.ResumeMovement();

            ClearLog();
        }

        public void ClearLog()
        {
            if (_logObject != null)
                Destroy(_logObject);

            _logObject = null;
            _hasLog = false;
        }

        protected override string InteractionText()
        {
            if (MinigameActive) return string.Empty;

            if (!_hasLog)
            {
                var held = GameObject.FindWithTag("Player")
                    ?.GetComponent<HeldObjectController>()?
                    .GetHeldObject();

                if (held is Pickable p && p.CompareTag("Log"))
                    return "Place Log (E)";

                return "Stump (Empty)";
            }

            // Has log → show chop interaction ONLY if holding axe
            var heldAxe = GameObject.FindWithTag("Player")
                ?.GetComponent<HeldObjectController>()?
                .GetHeldObject();

            if (heldAxe is Pickable axe && axe.CompareTag("Axe"))
                return "Start Chopping (E)";

            return "Stump (Occupied)";
        }
    }
}
