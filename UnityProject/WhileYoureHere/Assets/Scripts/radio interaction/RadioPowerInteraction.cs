using Interactable;
using UnityEngine;

namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour, IEInteractable,IClickInteractable
    {
        [SerializeField] private Canvas InteractiveCanvas;
        private RadioController radioController ;
        public void Start()
        {
            radioController = GetComponentInParent<RadioController>();
        }
    
        public override void Interact(IInteractor interactor)
        {
            switch (radioController.RadioOnStatus())
            {
                case false:
                    radioController.TurnRadioOn();
                    break;
                case true:
                    radioController.TurnRadioOff();
                    break;
            }
        }

        public override void ClickInteract(IInteractor interactor)
        {
            var isTuning = radioController.Tuning();
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

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);
            if (!radioController.RadioOnStatus())
            {
                InteractiveCanvas?.gameObject.SetActive(true);
            }
            
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);
            if (!radioController.RadioOnStatus())
            {
                InteractiveCanvas?.gameObject.SetActive(false);
            }        
        }
    }
}
