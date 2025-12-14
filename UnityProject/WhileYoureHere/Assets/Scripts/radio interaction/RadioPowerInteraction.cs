using Interactable;
using ScriptableObjects.dialogue;
using UnityEngine;

namespace radio_interaction
{
    public class RadioPowerInteraction : InteractableBehaviour, IClickInteractable,IEInteractable
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
        }

        public override void ClickInteract(IInteractor interactor)
        {
            radioController.OnTunePressed();
        }
    }
}