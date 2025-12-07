using Interactable;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DirtInteractable : InteractableBehaviour
{
    Renderer materialColor;
    [SerializeField] Color[] colorTransition;

    Color secondColor = new(255, 0, 0);
    [SerializeField] BroomScript broom;

    protected new void Awake()
    {
        base.Awake();
        materialColor = GetComponent<Renderer>();
    }

    public override void Interact(IInteractor interactor)
    {
        if (broom == null) { Debug.LogWarning("Broom can't be found!"); return; }

        if (!broom.IsHolding)
        {
            Debug.Log("Broom holding boolean is: " + broom.IsHolding);
            Debug.Log("You need to hold the broom for this!");
            return;
        }
        else if (broom.IsHolding)
        {
            Debug.Log("Broom holding boolean is: " + broom.IsHolding);
            Debug.Log("How nice of you to hold the broom before interacting!");
            materialColor.material.color = secondColor;
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
