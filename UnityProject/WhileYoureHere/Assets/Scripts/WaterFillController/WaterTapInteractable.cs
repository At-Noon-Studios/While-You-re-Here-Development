using Interactable;

namespace WaterFillController
{
    public class WaterTapInteractable : InteractableBehaviour
    {
        public WaterTap tap;

        protected override string InteractionText()
        {
            return tap.isRunning ? "Tap close" : "Tap open";
        }

        public override void Interact()
        {
            tap.ToggleTap();
        }
    }
}