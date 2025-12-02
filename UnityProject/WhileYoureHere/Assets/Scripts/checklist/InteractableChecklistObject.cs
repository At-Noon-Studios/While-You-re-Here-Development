using Interactable;
using UnityEngine;

namespace checklist
{
    public class InteractableChecklistObject : InteractableBehaviour
    {
        public ChecklistUI checklistUI;

        private bool _isPickedUp = false;

        public override void Interact(IInteractor interactor)
        {
            PickUpObject();
        }

        private void PickUpObject()
        {
            if (_isPickedUp) return;

            _isPickedUp = true;
            gameObject.SetActive(false);

            if (checklistUI != null)
            {
                checklistUI.SetChecklistAvailable(true, transform, gameObject);
            }
        }
    }
}