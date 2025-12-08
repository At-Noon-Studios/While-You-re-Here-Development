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
            radioController.OnPowerPressed();
        }

        public override void ClickInteract(IInteractor interactor)
        {
            radioController.OnTunePressed();
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);
           
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);
           
                InteractiveCanvas?.gameObject.SetActive(false);
                  
        }
    }
}
