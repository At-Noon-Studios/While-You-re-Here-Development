using Interactable;
using UnityEngine;

public class ScavengingScript : InteractableBehaviour
{
    // void Awake()
    // {
    //     base.Awake();
    // }

    void Update()
    {

    }

    public override void OnHoverEnter()
    {
        OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        OnHoverExit();
    }

    public override void Interact()
    {
        OnHoverExit();
        Debug.Log("You just interacted with a: " + gameObject.name);
        Destroy(gameObject);
    }
}
