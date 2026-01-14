using Interactable;
using UnityEngine;
using UnityEngine.Playables;

namespace TableLightLit
{
    public class TableLightInteraction : InteractableBehaviour
    {
        [SerializeField] private PlayableDirector director;
        
        protected override void Awake()
        {
            if (director == null)
                director = GetComponent<PlayableDirector>();
        }
    
        public override void Interact(IInteractor interactor)
        {
            print("I play director");
            director.Play();
        }
    }
}
