using System;
using Interactable;
using picking_up_objects;
using UnityEngine;

namespace chopping_logs
{
    public class LogChopTarget : InteractableBehaviour
    {
        [Header("Chopping settings")]
        [SerializeField] private GameObject choppedLogPrefab;
        [SerializeField] private Transform chopSpawnPoint1;
        [SerializeField] private Transform chopSpawnPoint2;

        private int _totalChopsRequired = 4;

        private int _currentChopCount;
        private Stump _stump;

        private void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _stump = GetComponentInParent<Stump>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Axe"))
            {
                RegisterHit();
            }
        }

        private void RegisterHit()
        {
            _currentChopCount++;
            Debug.Log($"Log hit! Current chop count: {_currentChopCount}/{_totalChopsRequired}");

            if (_currentChopCount >= _totalChopsRequired)
            {
                SplitLog();
                _stump.ClearLog();
            }
        }

        private void SplitLog()
        {
            if (_stump == null || !_stump.HasLog)
            {
                Debug.LogWarning("No log to chop on the stump.");
                return;
            }

            Instantiate(choppedLogPrefab, chopSpawnPoint1.position, chopSpawnPoint1.rotation);
            Instantiate(choppedLogPrefab, chopSpawnPoint2.position, chopSpawnPoint2.rotation);

            Debug.Log("Log chopped into pieces!");
        }

        protected override string InteractionText()
        {
            if (_stump != null && _stump.HasLog)
            {
                var player = GameObject.FindWithTag("Player");
                var heldController = player?.GetComponent<HeldObjectController>();
                var heldObject = heldController?.GetHeldObject();

                if (heldObject is Pickable pickableAxe && pickableAxe.CompareTag("Axe"))
                {
                    return "Start chopping the Log (E)";
                }
            }
            return string.Empty;
        }

        public override void Interact()
        {
            if (_stump != null && _stump.HasLog)
            {
                var player = GameObject.FindWithTag("Player");
                var heldController = player?.GetComponent<HeldObjectController>();
                var heldObject = heldController?.GetHeldObject();

                if (heldObject is Pickable pickableAxe && pickableAxe.CompareTag("Axe"))
                {
                    Debug.Log("Starting chopping minigame...");
                    _stump.StartMinigame();
                }
            }
        }
    }
}