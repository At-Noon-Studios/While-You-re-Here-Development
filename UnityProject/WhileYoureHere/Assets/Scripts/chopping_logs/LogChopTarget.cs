using System;
using UnityEngine;

namespace chopping_logs
{
    public class LogChopTarget : MonoBehaviour
    {
        [Header("Chopping settings")]
        [SerializeField] private GameObject choppedLogPrefab;
        [SerializeField] private Transform chopSpawnPoint1;
        [SerializeField] private Transform chopSpawnPoint2;

        [Header("Log settings")]
        [SerializeField] private int totalChopsRequired = 4;
        
        private int _currentChopCount;
        private Stump _stump;

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
            Debug.Log($"Log hit! Current chop count: {_currentChopCount}/{totalChopsRequired}");

            if (_currentChopCount >= totalChopsRequired)
            {
                SplitLog();
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

            Destroy(transform.root.gameObject);
            _stump.ClearLog();
            Debug.Log("Log chopped into pieces!");
        }
    }
}