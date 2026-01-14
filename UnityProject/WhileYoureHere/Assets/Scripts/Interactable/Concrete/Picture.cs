using Playable;
using UnityEngine;
using UnityEngine.Playables;

namespace Interactable.Concrete
{
    public class Picture : InteractableBehaviour
    {
        [SerializeField] private PlayableDirector director;
        private PlayableManager _playableManager;

        private void Start()
        {
            _playableManager = PlayableManager.Instance;
        }
        
        public override void Interact(IInteractor interactor)
        {
            _playableManager.Play(director);
        }
    }
}
