using Interactable;
namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour,
        IClickInteractable, IEInteractable
    {
        private RadioController _radioController;

        public void Awake()
        {
            base.Awake();
        }

        public void Start()
        {
            _radioController = GetComponentInParent<RadioController>();
        }

        public override void Interact(IInteractor interactor)
        {
            _radioController.OnPowerPressed();
        }

        public override void ClickInteract(IInteractor interactor)
        {
            _radioController.OnTunePressed();
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            bool interacted =
                _radioController.RadioStateMachine.CurrentState is RadioOffState || _radioController.RadioStateMachine.CurrentState is RadioOnState ;
            if (interacted)
                base.OnHoverEnter(interactor);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            bool interacted =
                _radioController.RadioStateMachine.CurrentState is RadioOffState || _radioController.RadioStateMachine.CurrentState is RadioOnState ;
            if (interacted)
                base.OnHoverExit(interactor);
        }

        public RadioController GetRadioController() => _radioController;
    }
}