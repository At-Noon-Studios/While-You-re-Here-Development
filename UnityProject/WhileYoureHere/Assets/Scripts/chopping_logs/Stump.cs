using Interactable;
using Interactable.Holdable;
using player_controls;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [SerializeField] private Transform logPlaceholder;
        [SerializeField] private Transform minigameStartPoint;

        public bool HasLog => _hasLog;
        public bool MinigameActive { get; private set; }

        private GameObject _logObject;
        private bool _hasLog;

        public override void Interact(IInteractor interactor)
        {
            if (MinigameActive)
            {
                Debug.Log("Interaction ignored: minigame already active.");
                return;
            }

            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<PlayerInteractionController>();
            var held = heldController?.HeldObject;

            if (!_hasLog)
            {
                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                {
                    Debug.Log("Placing log on stump.");
                    PlaceLog(pickableLog, heldController);
                }
                else
                {
                    Debug.Log("No log held to place.");
                }

                return;
            }

            if (held is HoldableObjectBehaviour p && p.GetComponentInChildren<AxeHitDetector>() != null)
            {
                Debug.Log("Starting chopping minigame.");
                StartMinigame();
            }
            else
            {
                Debug.Log("Held item is not a valid axe.");
            }
        }

        private void PlaceLog(HoldableObjectBehaviour pickableLog, PlayerInteractionController controller)
        {
            pickableLog.Place(logPlaceholder.position, logPlaceholder.rotation);

            var rb = pickableLog.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            pickableLog.enabled = false;

            _logObject = pickableLog.gameObject;
            _hasLog = true;

            var chopTargets = _logObject.GetComponentsInChildren<LogChopTarget>();
            foreach (var chopTarget in chopTargets)
            {
                chopTarget.SetStump(this);
            }

            controller.SetHeldObject(null);
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

            var player = GameObject.FindWithTag("Player");
            if (player != null && minigameStartPoint != null)
            {
                player.transform.position = minigameStartPoint.position;
                player.transform.rotation = minigameStartPoint.rotation;
            }

            var movement = player?.GetComponent<MovementController>();
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

        private void ClearLog()
        {
            if (_logObject != null)
                Destroy(_logObject);

            _logObject = null;
            _hasLog = false;
        }

        public override string InteractionText(IInteractor interactor)
        {
            if (MinigameActive) return string.Empty;

            var held = GameObject.FindWithTag("Player")
                ?.GetComponent<PlayerInteractionController>()
                ?.HeldObject;

            if (!_hasLog)
            {
                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                    return "Place Log (E)";

                return "Stump (Empty)";
            }

            if (held is HoldableObjectBehaviour && gameObject.GetComponentInChildren<AxeHitDetector>())
                return "Start Chopping (E)";

            return "Stump (Occupied)";
        }

        public bool IsReadyForChop()
        {
            return MinigameActive && _hasLog;
        }
    }
}