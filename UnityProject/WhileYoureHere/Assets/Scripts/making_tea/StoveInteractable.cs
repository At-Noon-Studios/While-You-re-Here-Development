using Interactable;

namespace making_tea
{
    public class StoveInteractable : InteractableBehaviour
    {
        public Stove stove;

        protected override string InteractionText()
        {
            if (stove == null) return string.Empty;
            return stove.isOn ? "Stove off" : "Stove on";
        }

        public override void Interact()
        {
            if (stove == null) return;
            stove.ToggleStove();
        }
    }
}