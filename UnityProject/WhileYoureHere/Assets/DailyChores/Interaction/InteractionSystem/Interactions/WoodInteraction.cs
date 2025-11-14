using UnityEngine;

public class WoodInteraction : MonoBehaviour, IInteractable
{
    private bool _hasInteracted = false;
    
    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log(Random.Range(0, 100));
        return true;
    }
}
