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
        if (!radioController.radioOn)
        {
            radioController.TurnRadioOn();
        }else if (radioController.radioOn)
        {
            // radioController.TurnRadioOff();
            print("radio on");
        }
        print(radioController.radioOn);
    }



}
