using Interactable;
using ScriptableObjects.Gamestate;
using UnityEngine;

namespace making_tea
{
    public class NotebookTablePickup : InteractableBehaviour, ITablePickup
    {
        [SerializeField] private SoGamestateFlag notebookPickedUpFlag;

        public bool IsTableHeld => false;

        protected override void Awake()
        {
            base.Awake();
            GameObject.FindWithTag("Player")
                ?.GetComponent<PlayerInteractionController>()
                ?.RegisterTablePickup(this);
        }

        public void Pickup(PlayerInteractionController pic)
        {
            BlockInteraction(true);

            pic.UnregisterTablePickup(this);

            gameObject.SetActive(false);

            if (notebookPickedUpFlag != null)
                notebookPickedUpFlag.currentValue = true;
        }

        public void Drop() { }
        public void ForceDropFromTableMode() { }

        public override void Interact(IInteractor interactor)
        {
            if (interactor is PlayerInteractionController { IsTableMode: true } pic)
                Pickup(pic);
        }

        public override bool IsInteractableBy(IInteractor interactor)
            => interactor is PlayerInteractionController { IsTableMode: true };
    }
}