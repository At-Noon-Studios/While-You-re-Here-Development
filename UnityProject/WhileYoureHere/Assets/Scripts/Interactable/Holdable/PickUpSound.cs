using UnityEngine;

namespace Interactable.Holdable
{
    public class PickUpSound : MonoBehaviour
    {
        [Header("Sound Settings")]
        [SerializeField] private AudioClip[] pickupSound;
        private AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayPickUpSound()
        {
            if (pickupSound != null)
            {
                _audioSource.PlayOneShot(pickupSound[Random.Range(0, pickupSound.Length - 1)]);
            }
        }
    }
}
