using Interactable;
using UnityEngine;

namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour,
        IClickInteractable, IEInteractable
    {
        [SerializeField] private Canvas InteractiveCanvas;
        private RadioController radioController;

        public void Start()
        {
            radioController = GetComponentInParent<RadioController>();
        }

        public override void Interact(IInteractor interactor)
        {
            radioController.OnPowerPressed();
            if (radioController.GetIsPlayingCutscene())
            {
                blockInteraction = true;
            }
            else blockInteraction = false;
        }

        public override void ClickInteract(IInteractor interactor)
        {
            if (radioController.GetIsPlayingCutscene())
            {
                blockInteraction = true;
                return;
            }
            blockInteraction = false;
            radioController.OnTunePressed();
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            bool interacted =
                radioController.RadioStateMachine.CurrentState is RadioOffState;
            base.OnHoverEnter(interactor);
            if (interacted) InteractiveCanvas.gameObject.SetActive(true);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);
            InteractiveCanvas.gameObject.SetActive(false);
        }
    }
}