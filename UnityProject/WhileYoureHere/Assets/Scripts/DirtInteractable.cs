using Interactable;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DirtInteractable : InteractableBehaviour
{
    Renderer materialColor;
    [SerializeField] Color[] colorTransition;

    Color secondColor = new(255, 0, 0);
    BroomScript broom;

    protected new void Awake()
    {
        base.Awake();
        materialColor = GetComponent<Renderer>();
        broom = GetComponent<BroomScript>();
    }

    public override void Interact(IInteractor interactor)
    {
        Debug.Log("status of holding the broom is: " + broom.IsHolding);
        if (broom.IsHolding)
        {
            materialColor.material.color = secondColor;
        }
        else
        {
            Debug.Log("You need to hold the broom for this!");
            return;
        }
        Debug.Log("You just interacted with a pile of shit... gross...");
        // ColorTransition();
    }

    public void ColorTransition()
    {
        for (int i = 0; i < colorTransition.Length; i++)
        {
            materialColor.material.color = colorTransition[i];
        }
    }
}
