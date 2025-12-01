using Interactable;

namespace making_tea
{
    public class StoveInteractable : InteractableBehaviour
    {
        public Stove stove;

        public override string InteractionText(IInteractor interactor)
        {
            if (stove == null) return string.Empty;
            return stove.isOn ? "Stove off" : "Stove on";
        }

        public override void Interact(IInteractor interactor)
        {
            if (stove == null) return;
            stove.ToggleStove();
        }
    }
}