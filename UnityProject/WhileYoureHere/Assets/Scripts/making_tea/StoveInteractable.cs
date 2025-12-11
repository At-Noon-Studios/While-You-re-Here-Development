using Interactable;
using UnityEngine;

namespace making_tea
{
    public class StoveInteractable : InteractableBehaviour
    {
        [Header("Stove Reference")]
        public Stove stove;

        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        private Transform _playerCamera;

        protected override void Awake()
        {
            base.Awake();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerCamera = player.GetComponentInChildren<Camera>()?.transform;
        }

        private void Update()
        {
            if (interactionCanvas == null ||
                !interactionCanvas.gameObject.activeSelf ||
                _playerCamera == null) return;
            interactionCanvas.transform.LookAt(_playerCamera);
            interactionCanvas.transform.Rotate(0f, 180f, 0f);
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(true);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor) => string.Empty;

        public override void Interact(IInteractor interactor)
        {
            if (stove == null) return;
            stove.ToggleStove();
        }
    }
}