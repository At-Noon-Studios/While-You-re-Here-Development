using System.Collections;
using chore;
using Interactable;
using Interactable.Holdable;
using player_controls;
using UnityEngine;
using UnityEngine.UI;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [Header("Minigame Settings")] 
        [SerializeField] private Transform logPlaceholder;

        [SerializeField] private Transform minigameStartPoint;

        [Header("UI References")]
        [SerializeField] private ChopUIManager uiManager;

        [Header("Sound Settings")] 
        [SerializeField] private AudioClip logPlaceSound;
        [SerializeField] private AudioClip logCrackSound;
        
        [Header("Sprite settings")]
        [SerializeField] private Image cutLogSprite;
        [SerializeField] private Image placeLogSprite;

        public static bool IsCurrentMinigameActive { get; private set; }
        public bool IsMinigameActive { get; private set; }

        private GameObject _logObject;
        private bool _hasLog;
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 0f;
            }
        }

        public override void Interact(IInteractor interactor)
        {
            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<PlayerInteractionController>();
            var held = heldController?.HeldObject;

            if (!_hasLog)
            {
                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                {
                    var chopTarget = pickableLog.GetComponentInChildren<LogChopTarget>();
                    if (chopTarget != null)
                    {
                        ChoreEvents.TriggerLogPlaced(chopTarget.GetLog());
                    }

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
                if (_audioSource != null && logPlaceSound != null)
                {
                    _audioSource.PlayOneShot(logPlaceSound);
                }

                chopTarget.SetStump(this);
            }

            controller.SetHeldObject(null);
        }


        private void StartMinigame()
        {
            if (!_hasLog) return;

            IsMinigameActive = true;
            IsCurrentMinigameActive = true;

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
            IsMinigameActive = false;
            IsCurrentMinigameActive = false;

            var player = GameObject.FindWithTag("Player");
            player.transform.SetParent(null);

            player?.GetComponent<MovementController>()?.ResumeMovement();

            ChopUIManager.Instance?.HideAllUI();
            StartCoroutine(ResumeCameraAfterDelay(2f));

            StartCoroutine(PlayCrackTwice(0.12f));

            var chopTarget = _logObject?.GetComponentInChildren<LogChopTarget>();

            ClearLog();

            if (chopTarget != null)
            {
                ChoreEvents.TriggerLogChopped(chopTarget.GetLog());
            }
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
            if (placeLogSprite != null)
                placeLogSprite.enabled = false;

            if (cutLogSprite != null)
                cutLogSprite.enabled = false;

            if (IsMinigameActive)
                return string.Empty;

            var held = GameObject.FindWithTag("Player")
                ?.GetComponent<PlayerInteractionController>()
                ?.HeldObject;

            if (!_hasLog)
            {
                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                {
                    if (placeLogSprite != null)
                        placeLogSprite.enabled = true;
                }

                return string.Empty;
            }

            if (held is HoldableObjectBehaviour h && h.GetComponentInChildren<AxeHitDetector>() != null)
            {
                if (cutLogSprite != null)
                    cutLogSprite.enabled = true;
            }

            return string.Empty;
        }



        public bool IsReadyForChop()
        {
            return IsMinigameActive && _hasLog;
        }

        private static IEnumerator ResumeCameraAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Camera.main?.GetComponent<CameraController>()?.ResumeCameraMovement();
            Debug.Log("Camera movement resumed after delay.");
        }

        private IEnumerator PlayCrackTwice(float delayBetween)
        {
            if (_audioSource == null || logCrackSound == null) yield break;

            _audioSource.PlayOneShot(logCrackSound);
            yield return new WaitForSeconds(delayBetween);
            _audioSource.PlayOneShot(logCrackSound);
        }
    }
}