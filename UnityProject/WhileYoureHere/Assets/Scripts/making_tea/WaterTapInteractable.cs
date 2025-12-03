using Interactable;

namespace making_tea
{
    public class WaterTapInteractable : InteractableBehaviour
    {
        public WaterTap tap;

        public override string InteractionText(IInteractor interactor)
        {
            return tap.isRunning ? "Tap close" : "Tap open";
        }

        public override void Interact(IInteractor interactor)
        {
            tap.ToggleTap();
        }
    }
}