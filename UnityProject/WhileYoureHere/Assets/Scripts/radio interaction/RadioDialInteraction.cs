using Interactable;
using UnityEngine;

public class RadioDialInteraction : InteractableBehaviour
{

    RadioController radioController ;
    public void Start()
    {
        radioController = GetComponentInParent<RadioController>();
        base.Start();
    }
    public override void Interact()
    {
        Debug.Log("interacted with dial");
        var isTuning = radioController.tuning();
        
        switch (isTuning)
        {
            case false:
                radioController.EnterTuningMode();
                break;
            case true:
                radioController.ExitTuningMode();
                break;
        }
        Debug.Log("interacted with dial" + isTuning);

    }
}
