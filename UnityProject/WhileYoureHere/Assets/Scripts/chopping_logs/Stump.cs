using Interactable;
using picking_up_objects;
using player_controls;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [SerializeField] private Transform logPlaceholder;

        public bool HasLog => _hasLog;
        public bool MinigameActive { get; private set; }

        private GameObject _logObject;
        private bool _hasLog;

        private void Awake() => base.Awake();

        public override void Interact()
        {
            if (MinigameActive)
            {
                Debug.Log("Interaction ignored: minigame already active.");
                return;
            }

            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<HeldObjectController>();
            var held = heldController?.GetHeldObject();

            if (!_hasLog)
            {
                if (held is Pickable pickableLog && pickableLog.CompareTag("Log"))
                {
                    Debug.Log("Placing log on stump.");
                    PlaceLog(pickableLog.GetComponent<Pickable>(), heldController);
                }
                else
                {
                    Debug.Log("No log held to place.");
                }

                return;
            }

            if (held is Pickable p && p.GetComponentInChildren<AxeHitDetector>() != null)
            {
                Debug.Log("Starting chopping minigame.");
                StartMinigame();
            }
            else
            {
                Debug.Log("Held item is not a valid axe.");
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
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            pickableLog.transform.SetParent(logPlaceholder);
            pickableLog.enabled = false;

            _logObject = pickableLog.gameObject;
            _hasLog = true;

            // Ensure LogChopTarget exists
            var chopTarget = _logObject.GetComponent<LogChopTarget>();
            if (chopTarget == null)
            {
                chopTarget = _logObject.AddComponent<LogChopTarget>();
            }
            chopTarget.SetStump(this);

            controller.ClearHeldObject();
            Debug.Log("Log placed on stump!");
        }

        public void StartMinigame()
        {
            if (!_hasLog)
            {
                Debug.LogWarning("Cannot start minigame: no log present.");
                return;
            }

            MinigameActive = true;
            Debug.Log("Minigame started!");

            var movement = GameObject.FindWithTag("Player")?.GetComponent<MovementController>();
            if (movement)
                movement.PauseMovement();
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

        // ReSharper disable Unity.PerformanceAnalysis
        protected override string InteractionText()
        {
            if (MinigameActive) return string.Empty;

            var held = GameObject.FindWithTag("Player")
                ?.GetComponent<HeldObjectController>()
                ?.GetHeldObject();

            if (!_hasLog)
            {
                if (held is Pickable pickableLog && pickableLog.CompareTag("Log"))
                    return "Place Log (E)";

                return "Stump (Empty)";
            }

            if (held is Pickable pick && pick.GetComponentInChildren<AxeHitDetector>())
                return "Start Chopping (E)";

            return "Stump (Occupied)";
        }

        public bool IsReadyForChop()
        {
            return MinigameActive && _hasLog;
        }
    }
}