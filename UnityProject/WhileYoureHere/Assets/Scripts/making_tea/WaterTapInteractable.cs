using Interactable;
using UnityEngine;

namespace making_tea
{
    public class WaterTapInteractable : InteractableBehaviour
    {
        [Header("Water Tap Reference")]
        [SerializeField] private WaterTap tap;

        public override string InteractionText(IInteractor interactor) => string.Empty;

        public override void Interact(IInteractor interactor)
        {
            if (tap == null)
            {
                Debug.LogWarning("WaterTapInteractable: WaterTap reference is missing!");
                return;
            }

            tap.ToggleTap();
        }
    }
}