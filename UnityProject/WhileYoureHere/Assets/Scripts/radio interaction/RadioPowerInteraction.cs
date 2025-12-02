using Interactable;

namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour, IEInteractable
    {

        RadioController radioController ;
        public void Start()
        {
            radioController = GetComponentInParent<RadioController>();
            // base.Start();
        }
    
        public override void Interact(IInteractor interactor)
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
            print(radioController.radioOn);    }
    }
}
