using chore;
using Interactable;
using UnityEngine;

namespace entity
{
    [RequireComponent(typeof(AudioSource))]
    public class Item : InteractableBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] ScavengingChore sc;
        [SerializeField] float audioVolume = 1.0f;

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
            AudioManager.instance.PlaySound(sc.PickupPlants, transform, audioVolume);
            Destroy(gameObject);
        }
    }
}
