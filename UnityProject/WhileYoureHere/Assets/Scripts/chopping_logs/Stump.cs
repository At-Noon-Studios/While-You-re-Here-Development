using Interactable;
using Interactable.Holdable;
using player_controls;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [Header("Minigame Settings")]
        [SerializeField] private Transform logPlaceholder;
        [SerializeField] private Transform minigameStartPoint;
        
        [Header("UI References")]
        [SerializeField] private ChopUIManager uiManager;

        public static bool CurrentMinigameActive { get; private set; }

        public bool HasLog => _hasLog;
        public bool MinigameActive { get; private set; }

        private GameObject _logObject;
        private bool _hasLog;
        
        public override void Interact(IInteractor interactor)
        {
            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<PlayerInteractionController>();
            var held = heldController?.HeldObject;

            if (!_hasLog)
            {
                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                {
                    PlaceLog(pickableLog, heldController);
                }
            }

            if (held is HoldableObjectBehaviour p && p.GetComponentInChildren<AxeHitDetector>() != null)
            {
                StartMinigame();
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

            var holdable = pickableLog.GetComponent<HoldableObjectBehaviour>();
            if (holdable != null)
            {
                holdable.enabled = false;
                Debug.Log($"HoldableObjectBehaviour disabled for log: {pickableLog.name}");
            }

            _logObject = pickableLog.gameObject;
            _hasLog = true;

            var chopTargets = _logObject.GetComponentsInChildren<LogChopTarget>();
            foreach (var chopTarget in chopTargets)
            {
                chopTarget.SetStump(this);
            }

            controller.SetHeldObject(null);
        }


        private void StartMinigame()
        {
            if (!_hasLog) return;

            MinigameActive = true;
            CurrentMinigameActive = true;

            var camera = GameObject.FindWithTag("MainCamera");
            var player = GameObject.FindWithTag("Player");
            if (player != null && minigameStartPoint != null)
            {
                player.transform.SetParent(minigameStartPoint);
                
                player.transform.position = minigameStartPoint.position;
                player.transform.rotation = minigameStartPoint.rotation;
                camera.transform.rotation = minigameStartPoint.rotation;
                
            }

            player?.GetComponent<MovementController>()?.PauseMovement();

            Camera.main?.GetComponent<CameraController>()?.PauseCameraMovement();

            ChopUIManager.Instance?.ShowUI();
            
            var axeDetector = player.GetComponentInChildren<AxeHitDetector>();
            if (axeDetector != null)
            {
                axeDetector.SetBaseRotation();
                Debug.Log("Base rotation set at minigame start.");
            }
        }

        public void EndMinigame()
        {
            MinigameActive = false;
            CurrentMinigameActive = false;

            var player = GameObject.FindWithTag("Player");
            player.transform.SetParent(null);

            player?.GetComponent<MovementController>()?.ResumeMovement();
            Camera.main?.GetComponent<CameraController>()?.ResumeCameraMovement();

            ChopUIManager.Instance?.HideAllUI();

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

            if (held is HoldableObjectBehaviour h && h.GetComponentInChildren<AxeHitDetector>() != null)
                return "Start Chopping (E)";

            return "Stump (Occupied)";
        }

        public bool IsReadyForChop()
        {
            return MinigameActive && _hasLog;
        }
    }
}
