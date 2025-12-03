using Interactable;
using UnityEngine;

namespace checklist
{
    public class InteractableChecklistObject : InteractableBehaviour
    {
        [SerializeField] private ChecklistUI checklistUI;

        private bool _isPickedUp;

        public override void Interact(IInteractor interactor)
        {
            if (_isPickedUp) return;

            _isPickedUp = true;
            gameObject.SetActive(false);
            checklistUI.SetChecklistAvailable(true, transform, gameObject);
        }
    }
}
