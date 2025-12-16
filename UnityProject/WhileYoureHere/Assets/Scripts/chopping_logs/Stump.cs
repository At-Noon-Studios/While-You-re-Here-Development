using System.Collections;
using chore;
using Interactable;
using Interactable.Holdable;
using player_controls;
using PlayerControls;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace chopping_logs
{
    public class Stump : InteractableBehaviour
    {
        [Header("Minigame Settings")] 
        [SerializeField] private Transform logPlaceholder;
        [SerializeField] private EventChannel cancelEvent;
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

        private void OnEnable() => cancelEvent.OnRaise += OnCancelInput;
        private void OnDisable() => cancelEvent.OnRaise -= OnCancelInput;

        private void OnCancelInput()
        {
            if (IsCurrentMinigameActive && IsMinigameActive)
                EndMinigame();
        }
        
        public override void Interact(IInteractor interactor)
        {
            var player = GameObject.FindWithTag("Player");
            var heldController = player?.GetComponent<PlayerInteractionController>();
            var held = heldController?.HeldObject;
            
            if (_hasLog && held == null)
            {
                TakeLog(heldController);
                return;
            }
            
            if (!_hasLog)
            {
                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                {
                    var chopTarget = pickableLog.GetComponentInChildren<LogChopTarget>();
                    if (chopTarget != null)
                        ChoreEvents.TriggerLogPlaced(chopTarget.GetLog());

                    PlaceLog(pickableLog, heldController);
                    return;
                }
            }
            
            if (_hasLog && held is HoldableObjectBehaviour h &&
                h.GetComponentInChildren<AxeHitDetector>() != null)
            {
                StartMinigame();
            }
        }
        
        private void TakeLog(PlayerInteractionController player)
        {
            if (!_hasLog || _logObject == null) return;

            var holdable = _logObject.GetComponent<HoldableObjectBehaviour>();
            if (holdable == null) return;

            holdable.PickUpByInteractor(player);

            _logObject = null;
            _hasLog = false;
        }


        private void PlaceLog(HoldableObjectBehaviour pickableLog, PlayerInteractionController controller)
        {
            pickableLog.Place(logPlaceholder.position, logPlaceholder.rotation);

            var rb = pickableLog.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
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

        public void OnLogPickedUp(GameObject pickedLog)
        {
            if (_logObject == pickedLog)
            {
                _logObject = null;
                _hasLog = false;
            }
        }
        
        private void StartMinigame()
        {
            if (!_hasLog) return;

            IsMinigameActive = true;
            IsCurrentMinigameActive = true;

            var player = GameObject.FindWithTag("Player");
            var cameraController = Camera.main?.GetComponent<CameraController>();

            if (player != null)
            {
                player.transform.SetPositionAndRotation(
                    minigameStartPoint.position,
                    minigameStartPoint.rotation
                );

                player.GetComponent<MovementController>()?.PauseMovement();
            }
            
            cameraController?.SetMinigameRotation(minigameStartPoint.rotation);
            cameraController?.PauseCameraMovement();

            ChopUIManager.Instance?.ShowUI();
            player.GetComponentInChildren<AxeHitDetector>()?.SetBaseRotation();
        }
        
        public void EndMinigame()
        {
            IsMinigameActive = false;
            IsCurrentMinigameActive = false;

            var player = GameObject.FindWithTag("Player");
            var cameraController = Camera.main?.GetComponent<CameraController>();

            player.GetComponent<MovementController>()?.ResumeMovement();
            
            cameraController?.SyncRotation(Camera.main.transform.rotation);
            cameraController?.ResumeCameraMovement();

            ChopUIManager.Instance?.HideAllUI();
            StartCoroutine(PlayCrackTwice(0.12f));

            var chopTarget = _logObject?.GetComponentInChildren<LogChopTarget>();
            ClearLog();

            if (chopTarget != null)
                ChoreEvents.TriggerLogChopped(chopTarget.GetLog());
            
            var playerController = player.GetComponent<PlayerInteractionController>();
            var heldBehaviour = playerController?.HeldObject as HoldableObjectBehaviour;
            heldBehaviour?.ResetPose();
        }

        private void ClearLog()
        {
            if (_logObject != null) Destroy(_logObject);

            _logObject = null;
            _hasLog = false;
        }

        public override string InteractionText(IInteractor interactor)
        {
            HideInteractionSprites();

            if (IsMinigameActive)
                return string.Empty;

            var held = GameObject.FindWithTag("Player")
                ?.GetComponent<PlayerInteractionController>()
                ?.HeldObject;

            if (!_hasLog)
            {
                if (!_hasLog && held is HoldableObjectBehaviour pickableLog && pickableLog != null)
                {
                    if (pickableLog.CompareTag("Log") && placeLogSprite != null)
                        placeLogSprite.enabled = true;

                    return string.Empty;
                }
            }

            if (held is HoldableObjectBehaviour h && h.GetComponentInChildren<AxeHitDetector>() != null)
            {
                if (cutLogSprite != null)
                    cutLogSprite.enabled = true;
            }

            return string.Empty;
        }

        public override void OnHoverExit(IInteractor interactor) => HideInteractionSprites();

        private void HideInteractionSprites()
        {
            if (placeLogSprite != null) placeLogSprite.enabled = false;
            if (cutLogSprite != null) cutLogSprite.enabled = false;
        }
        
        public bool IsReadyForChop() => IsMinigameActive && _hasLog;

        private IEnumerator PlayCrackTwice(float delayBetween)
        {
            if (_audioSource == null || logCrackSound == null) yield break;

            _audioSource.PlayOneShot(logCrackSound);
            yield return new WaitForSeconds(delayBetween);
            _audioSource.PlayOneShot(logCrackSound);
        }
    }
}