using Interactable;
using UnityEngine;

public class RadioDialInteraction : InteractableBehaviour
{

    RadioController radioController ;

    public void Start()
    {
        radioController = GetComponentInParent<RadioController>();
    }
    public override void Interact()
    {
        radioController.EnterTuningMode();
    }
}
