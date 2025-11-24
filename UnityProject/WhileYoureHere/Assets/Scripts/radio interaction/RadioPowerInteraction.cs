using System.Collections;
using Interactable;
using UnityEngine;

public class RadioPowerInteraction : InteractableBehaviour
{

    RadioController radioController ;
    public void Start()
    {
        radioController = GetComponentInParent<RadioController>();
        base.Start();
    }
    public override void Interact()
    {
        print("interacted with RadioPowerInteraction");
        switch (radioController.radioOn)
        {
            case false:
                radioController.TurnRadioOn();
                break;
            case true:
                radioController.TurnRadioOff();
                break;
        }
        print(radioController.radioOn);
    }



}
