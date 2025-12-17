using Interactable;

namespace making_tea
{
    public interface ITablePickup : IInteractable
    {
        bool IsTableHeld { get; }
        void Pickup(PlayerInteractionController pic);
        void Drop();
        void ForceDropFromTableMode();
    }
}