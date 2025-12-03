using Interactable;
using UnityEngine;

namespace chore.scavenging
{
    [RequireComponent(typeof(AudioSource))]
    public class Item : InteractableBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private ScavengingChore _scavengeChore;
        [SerializeField] private float _audioVolume = 1.0f;

        [Header("Item")]
        [SerializeField] private int itemID;

        private new void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }

        public override void Interact(IInteractor interactor)
        {
            ChoreEvents.TriggerItemCollected(itemID);
            AudioManager.instance.PlaySound(_scavengeChore.PickupPlants, transform, _audioVolume);
            Destroy(gameObject);
        }
        public override string InteractionText(IInteractor interactor) => "";
    }
}