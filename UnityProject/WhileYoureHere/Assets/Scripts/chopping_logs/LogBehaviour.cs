using Interactable;
using picking_up_objects;
using UnityEngine;

namespace chopping_logs
{
    public class LogBehaviour : Pickable
    {
        [Header("Log Settings")]
        [SerializeField] private float damagePerHit = 1f;
        [SerializeField] private int maxHits = 3;
        [SerializeField] private GameObject splitLogPrefab;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip crackSound;
        [SerializeField] private AudioClip chopSound;

        private int _currentHits;
        private bool _isOnStump;

        private void Awake()
        {
            base.Awake();
            _currentHits = 0;
        }

        /// <summary>
        /// Called when player interacts directly with the log (e.g. pickup).
        /// </summary>
        public override void Interact()
        {
            Debug.Log("Interacted with log");
            base.Interact();
            // Could trigger pickup logic here if desired
        }

        /// <summary>
        /// Called when the axe hits the log.
        /// </summary>
        public void GetLogHit()
        {
            _currentHits++;
            Debug.Log($"Log hit! Current hits: {_currentHits}");

            if (audioSource != null && chopSound != null)
                audioSource.PlayOneShot(chopSound);

            if (_currentHits >= maxHits)
            {
                SplitLog();
            }
        }

        /// <summary>
        /// Splits the log into two smaller pieces with physics forces.
        /// </summary>
        private void SplitLog()
        {
            if (splitLogPrefab == null)
            {
                Debug.LogWarning("No splitLogPrefab assigned!");
                Destroy(gameObject);
                return;
            }

            // Left piece
            GameObject leftLog = Instantiate(splitLogPrefab, transform.position + Vector3.left * 0.3f, Quaternion.identity);
            Rigidbody rbLeft = leftLog.GetComponent<Rigidbody>();
            if (rbLeft != null)
                rbLeft.AddForce(Vector3.left * 3f + Vector3.up * 2f, ForceMode.Impulse);

            // Right piece
            GameObject rightLog = Instantiate(splitLogPrefab, transform.position + Vector3.right * 0.3f, Quaternion.identity);
            Rigidbody rbRight = rightLog.GetComponent<Rigidbody>();
            if (rbRight != null)
                rbRight.AddForce(Vector3.right * 3f + Vector3.up * 2f, ForceMode.Impulse);

            // Play crack sound
            if (audioSource != null && crackSound != null)
                audioSource.PlayOneShot(crackSound);

            Debug.Log("Log split into two pieces!");

            // Destroy original log
            Destroy(gameObject);
        }

        /// <summary>
        /// Marks the log as placed on stump.
        /// </summary>
        public void PlaceOnStump(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            _isOnStump = true;
        }

        /// <summary>
        /// Returns whether the log is currently placed on stump.
        /// </summary>
        public bool IsOnStump => _isOnStump;
    }
}