using chore;
using UnityEngine;

namespace chopping_logs
{
    public class LogChopTarget : MonoBehaviour
    {
        private const int TotalHits = 3;
        private int _hits;
        private Stump _stump;

        [Header("Chopped Log Settings")]
        [SerializeField] private GameObject choppedLogQuarterPrefab;
        [SerializeField] private Transform[] spawnPoints = new Transform[2];
        
        [Header("Chore ID reference")]
        [SerializeField] private int logID;
        
        
        
        private void Start()
        {
            _stump = GetComponentInParent<Stump>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_stump == null) return;
            
            if (!_stump.IsReadyForChop()) return;

            var axe = other.GetComponent<AxeHitDetector>()
                      ?? other.GetComponentInParent<AxeHitDetector>()
                      ?? other.GetComponentInChildren<AxeHitDetector>();

            if (axe == null) return;

            RegisterHit();
        }

        private void RegisterHit()
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
            if (choppedLogQuarterPrefab == null) { return; }

            for (var i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] != null)
                {
                    Instantiate(choppedLogQuarterPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                }
            }
            _hits = 0;
            _stump.EndMinigame();
        }
        
        public void SetStump(Stump stump) => _stump = stump;

        public int GetLog() => logID;
    }
}