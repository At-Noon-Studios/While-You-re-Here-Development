using chore;
using Interactable.Holdable;
using UnityEngine;


namespace Interactable.Concrete
{
    [RequireComponent(typeof(AudioSource))]
    public class Newspaper : HoldableObjectBehaviour
    {
        private const int ItemID = 5;
        private AudioSource _audioSource;
        
        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public override void Interact(IInteractor interactor)
        {
            ChoreEvents.TriggerItemCollected(ItemID);
            base.Interact(interactor);
            _audioSource.Play();
        }
    }
}