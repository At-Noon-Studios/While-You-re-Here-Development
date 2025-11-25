using UnityEngine;
using picking_up_objects;

namespace chopping_logs
{
    public class AxeBehaviour : MonoBehaviour
    {
        [Header("Axe State")]
        public bool IsHeld { get; private set; }

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip dropSound;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Called by your pickup system when the player grabs the axe.
        /// </summary>
        public void OnPickup()
        {
            IsHeld = true;

            if (_rb != null)
            {
                _rb.useGravity = false;
                _rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (audioSource != null && pickupSound != null)
                audioSource.PlayOneShot(pickupSound);
        }

        /// <summary>
        /// Called when the axe is dropped.
        /// </summary>
        public void OnDrop()
        {
            IsHeld = false;

            if (_rb != null)
            {
                _rb.useGravity = true;
                _rb.constraints = RigidbodyConstraints.None;
            }

            if (audioSource != null && dropSound != null)
                audioSource.PlayOneShot(dropSound);
        }
    }
}