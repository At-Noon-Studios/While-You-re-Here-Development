using Interactable;

namespace door
{
    public class Keyhole : InteractableBehaviour
    {
        [Header("Door Lock")]
        [SerializeField] private bool isLocked = false;
        
        public override void Interact(IInteractor interactor)
        {
            throw new System.NotImplementedException();
        }
    }
}