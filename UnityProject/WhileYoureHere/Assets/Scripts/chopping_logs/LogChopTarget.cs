using UnityEngine;

namespace chopping_logs
{
    public class LogChopTarget : MonoBehaviour
    {
        private const int TotalHits = 4;
        private int _hits;
        private Stump _stump;

        [SerializeField] private GameObject choppedLogPrefab;
        [SerializeField] private Transform spawn1;
        [SerializeField] private Transform spawn2;

        private void Start()
        {
            _stump = GetComponentInParent<Stump>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_stump || !_stump.MinigameActive || !_stump.HasLog) return;

            if (other.CompareTag("Axe"))
            {
                Debug.Log("Trigger hit by AXE: " + other.name);

                var axe = other.GetComponent<AxeHitDetector>();
                if (axe == null)
                {
                    Debug.Log("Axe has no AxeHitDetector!");
                    return;
                }

                RegisterHit();
            }
        }

        private void RegisterHit()
        {
            _hits++;
            Debug.Log($"HIT {_hits}/{TotalHits}");

            if (_hits >= TotalHits)
            {
                ChopLog();
            }
        }

        private void ChopLog()
        {
            Instantiate(choppedLogPrefab, spawn1.position, spawn1.rotation);
            Instantiate(choppedLogPrefab, spawn2.position, spawn2.rotation);

            Debug.Log("LOG CHOPPED!");

            _hits = 0;
            _stump.EndMinigame(); // ends the minigame + clears stump
        }
    }
}