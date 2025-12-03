using Interactable;

namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour, IEInteractable
    {
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
    }
}
