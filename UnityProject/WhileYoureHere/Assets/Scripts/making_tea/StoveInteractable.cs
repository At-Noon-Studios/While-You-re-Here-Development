using Interactable;
using UnityEngine;

namespace making_tea
{
    public class StoveInteractable : InteractableBehaviour
    {
        [Header("Stove Reference")]
        public Stove stove;

        public override string InteractionText(IInteractor interactor) => string.Empty;

        public override void Interact(IInteractor interactor)
        {
            if (stove == null)
            {
                Debug.LogWarning("StoveInteractable: Stove reference is missing!");
                return;
            }
            stove.ToggleStove();
        }
    }
}