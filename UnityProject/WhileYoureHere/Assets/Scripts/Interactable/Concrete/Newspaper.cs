using chore;
using Interactable.Holdable;
using UnityEngine;


namespace Interactable.Concrete
{
    [RequireComponent(typeof(AudioSource))]
    public class Newspaper : HoldableObjectBehaviour
    {
        private AudioSource _audioSource;
        
        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public override void Interact(IInteractor interactor)
        {
            base.Interact(interactor);
            _audioSource.Play();
        }
    }
}