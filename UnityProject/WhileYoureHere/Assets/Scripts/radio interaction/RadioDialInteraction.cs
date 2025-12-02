using Interactable;
using UnityEngine;

namespace radio_interaction
{
    public class RadioDialInteraction : InteractableBehaviour, IClickInteractable
    {

        RadioController radioController ;
        public void Start()
        {
            radioController = GetComponentInParent<RadioController>();
            // base.Start();
        }

        public override void Interact(IInteractor interactor)
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
}
