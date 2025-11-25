using Interactable;
using UnityEngine;

public class ScavengingScript : InteractableBehaviour
{
    void Awake()
    {
        base.Awake();
    }

    void Update()
    {

    }

    public override void OnHoverEnter(string objectname)
    {
        OnHoverEnter(objectname);
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
    }

    public override void Interact()
    {
        base.OnHoverExit();
        Debug.Log("You just interacted with a: " + gameObject.name);
        Destroy(gameObject);
    }
}
