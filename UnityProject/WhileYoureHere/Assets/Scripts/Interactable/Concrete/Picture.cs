using Interactable;
using UnityEngine;
using UnityEngine.Playables;

public class Picture : InteractableBehaviour
{
    [SerializeField] private PlayableDirector director;
    
    
    
    public override void Interact(IInteractor interactor)
    {
        director.Play();
    }
}
