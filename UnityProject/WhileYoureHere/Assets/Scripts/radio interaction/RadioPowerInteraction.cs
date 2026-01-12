using Interactable;
using UnityEngine;

namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour,
        IClickInteractable, IEInteractable
    {
        [SerializeField] private Canvas InteractiveCanvas;
        private RadioController _radioController;

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

        // public override void OnHoverEnter(IInteractor interactor)
        // {
        //     bool interacted = _radioController.RadioStateMachine.CurrentState is RadioOffState;
        //     base.OnHoverEnter(interactor);
        //     if (interacted) InteractiveCanvas.gameObject.SetActive(true);
        // }
        //
        // public override void OnHoverExit(IInteractor interactor)
        // {
        //     base.OnHoverEnter(interactor);
        //     InteractiveCanvas.gameObject.SetActive(false);
        // }
        
        public RadioController GetRadioController() => _radioController;
    }
}