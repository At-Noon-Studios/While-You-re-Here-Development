using UnityEngine;

namespace chopping_logs
{
    public class LogChopTarget : MonoBehaviour
    {
        private const int TotalHits = 4;
        private int _hits = 0;
        private Stump _stump;

        public GameObject choppedLogPrefab;
        public Transform spawn1;
        public Transform spawn2;

        private void Start()
        {
            _stump = GetComponentInParent<Stump>();
            Debug.Log($"LogChopTarget initialized. Stump found: {_stump != null}");
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Trigger entered by: {other.name}");

            if (_stump == null)
            {
                Debug.LogWarning("No stump reference in LogChopTarget.");
                return;
            }

            Debug.Log($"Stump state → MinigameActive: {_stump.MinigameActive}, HasLog: {_stump.HasLog}");

            if (!_stump.IsReadyForChop())
            {
                Debug.Log("Hit ignored: stump inactive or no log.");
                return;
            }

            var axe = other.GetComponent<AxeHitDetector>()
                      ?? other.GetComponentInParent<AxeHitDetector>()
                      ?? other.GetComponentInChildren<AxeHitDetector>();

            if (axe == null)
            {
                Debug.Log("No AxeHitDetector found.");
                return;
            }

            Debug.Log("Valid hit detected!");
            axe.ConsumeHit();
            RegisterHit();
        }

        private void RegisterHit()
        {
            _hits++;
            Debug.Log($"LOG HIT: {_hits}/{TotalHits}");

            if (_hits >= TotalHits)   // ✅ fix threshold
            {
                ChopLog();
            }
        }

        private void ChopLog()
        {
            if (choppedLogPrefab != null)
            {
                if (spawn1 != null)
                    Instantiate(choppedLogPrefab, spawn1.position, spawn1.rotation);
                if (spawn2 != null)
                    Instantiate(choppedLogPrefab, spawn2.position, spawn2.rotation);
            }
            else
            {
                Debug.LogWarning("No choppedLogPrefab assigned!");
            }

            Debug.Log("LOG CHOPPED!");

            _hits = 0;
            _stump.EndMinigame();   // ✅ this will Destroy(_logObject)
        }


        public void SetStump(Stump stump) => _stump = stump;
    }
}