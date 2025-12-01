using UnityEngine;

namespace chopping_logs
{
    public class LogChopTarget : MonoBehaviour
    {
        private const int TotalHits = 4;
        private int _hits = 0;
        private Stump _stump;

        public GameObject choppedLogQuarterPrefab;
        public Transform[] spawnPoints = new Transform[4];

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

        public void RegisterHit()
        {
            _hits++;
            Debug.Log($"LOG HIT: {_hits}/{TotalHits}");

            if (_hits >= TotalHits)
            {
                ChopLog();
            }
        }

        private void ChopLog()
        {
            if (choppedLogQuarterPrefab == null)
            {
                Debug.LogWarning("No choppedLogQuarterPrefab assigned!");
                return;
            }

            for (var i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] != null)
                {
                    Instantiate(choppedLogQuarterPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                }
                else
                {
                    Debug.LogWarning($"Missing spawn point {i} for chopped log quarter.");
                }
            }

            Debug.Log("LOG CHOPPED INTO QUARTERS!");
            _hits = 0;
            _stump.EndMinigame();
        }


        public void SetStump(Stump stump) => _stump = stump;
    }
}