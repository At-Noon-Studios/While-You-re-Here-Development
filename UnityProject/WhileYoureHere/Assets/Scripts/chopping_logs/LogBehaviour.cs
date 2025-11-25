using UnityEngine;

namespace chopping_logs
{
    public class LogBehaviour : MonoBehaviour
    {
        [Header("Log Settings")]
        [SerializeField] private int maxHits = 3;
        [SerializeField] private float splitForce = 3f;
        [SerializeField] private float upwardForce = 2f;
        [SerializeField] private GameObject splitLogPrefab;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip crackSound;    // cracking when splitting
        [SerializeField] private AudioClip chopSound;     // chop impact sound per hit

        private int _currentHits = 0;
        private bool _isOnStump = false;
        private Stump _stump;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Called by stump when log is snapped onto it.
        /// </summary>
        public void PlaceOnStump(Stump stump)
        {
            _isOnStump = true;
            _stump = stump;

            // Freeze log when properly placed
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.useGravity = false;
                _rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        /// <summary>
        /// Called when axe correctly hits log.
        /// </summary>
        public void GetLogHit()
        {
            _currentHits++;

            // Chop sound for each hit
            if (audioSource != null && chopSound != null)
                audioSource.PlayOneShot(chopSound);

            Debug.Log($"Log hit ({_currentHits}/{maxHits})");

            if (_currentHits >= maxHits)
                SplitLog();
        }

        /// <summary>
        /// Splits the log into two halves with physics forces.
        /// </summary>
        private void SplitLog()
        {
            if (splitLogPrefab == null)
            {
                Debug.LogWarning("Split log prefab missing!");
                Destroy(gameObject);
                return;
            }

            // Unfreeze the original log (so halves can fly correctly)
            if (_rb != null)
                _rb.constraints = RigidbodyConstraints.None;

            // Tell stump the log is gone
            if (_stump != null)
                _stump.ClearLog();

            // Play crack sound
            if (audioSource != null && crackSound != null)
                audioSource.PlayOneShot(crackSound);

            Debug.Log("LOG SPLITTING!");

            // Create left piece
            var leftPiece = Instantiate(
                splitLogPrefab,
                transform.position + Vector3.left * 0.3f,
                Quaternion.identity
            );
            var leftRb = leftPiece.GetComponent<Rigidbody>();
            if (leftRb != null)
            {
                leftRb.AddForce(
                    Vector3.left * splitForce + Vector3.up * upwardForce,
                    ForceMode.Impulse
                );
            }

            // Create right piece
            var rightPiece = Instantiate(
                splitLogPrefab,
                transform.position + Vector3.right * 0.3f,
                Quaternion.identity
            );
            var rightRb = rightPiece.GetComponent<Rigidbody>();
            if (rightRb != null)
            {
                rightRb.AddForce(
                    Vector3.right * splitForce + Vector3.up * upwardForce,
                    ForceMode.Impulse
                );
            }

            // Destroy original log
            Destroy(gameObject);
        }

        public bool IsOnStump => _isOnStump;
    }
}
