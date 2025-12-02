using Interactable;
using UnityEngine;

namespace radio_interaction
{
    public class RadioDialInteraction : InteractableBehaviour, IClickInteractable
    {
        private RadioController radioController;

        public void Start()
        {
            radioController = GetComponentInParent<RadioController>();
        }

        public override void Interact(IInteractor interactor)
        {
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
        }
    }
}
