using chore;
using Interactable;
using UnityEngine;

namespace entity
{
    [RequireComponent(typeof(AudioSource))]
    public class Item : InteractableBehaviour
    {
        private AudioSource _audioSource;
        // [SerializeField] AudioClip pickupSound;

        [SerializeField] ScavengingChore sc;

        [Header("Item")]
        [SerializeField] private int itemID;

        private new void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }

        public override void Interact()
        {
            ChoreEvents.TriggerItemCollected(itemID);
            _audioSource.clip = sc.PickupPlants;
            _audioSource.Play();
            Debug.Log("Audioclip " + _audioSource.clip + " was played just now!");

            // this line here is my enemy :(
            // Destroy(gameObject);
        }
    }
}
