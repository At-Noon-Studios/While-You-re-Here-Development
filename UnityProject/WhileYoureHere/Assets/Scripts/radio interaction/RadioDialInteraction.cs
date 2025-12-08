using Interactable;
using UnityEngine;

namespace radio_interaction
{
    public class RadioDialInteraction : InteractableBehaviour, IClickInteractable
    {
        [SerializeField] Canvas InteractiveCanvas;
        private RadioController radioController;
        public void Start()
        {
            radioController = GetComponentInParent<RadioController>();
        }

        public override void Interact(IInteractor interactor)
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

        public override void ClickInteract(IInteractor interactor)
        {
            
        }

        // public override void OnHoverEnter(IInteractor interactor)
        // {
        //     base.OnHoverEnter(interactor);
        //     InteractiveCanvas?.gameObject.SetActive(true);
        // }
        //
        // public override void OnHoverExit(IInteractor interactor)
        // {
        //      base.OnHoverExit(interactor);
        //      InteractiveCanvas?.gameObject.SetActive(false);
        // }
    }
}
