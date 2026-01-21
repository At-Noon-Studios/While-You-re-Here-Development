using Interactable;
using UnityEngine;
using UnityEngine.Playables;

public class InteractionCutscene : InteractableBehaviour
{
    [SerializeField] private PlayableDirector director;
        
    protected override void Awake()
    {
        base.Awake();
            
        if (director == null)
            director = GetComponent<PlayableDirector>();
    }
    
    public override void Interact(IInteractor interactor)
    {
        director.Play();
    }
}
