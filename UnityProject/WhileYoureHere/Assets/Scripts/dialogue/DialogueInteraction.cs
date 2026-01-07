using UnityEngine;
using Interactable;
using player_controls;
using PlayerControls;
using ScriptableObjects.dialogue;
using UnityEngine.Playables;

namespace dialogue
{
    public class DialogueInteraction : InteractableBehaviour, IEInteractable
    {
        [Header("Interaction")] 
        [SerializeField] private Canvas interactionCanvas;
        [SerializeField] private GameObject interactionUI;

        [Header("Dialogue")] 
        [SerializeField] private DialogueLoader dialogueLoader;
        [SerializeField] private DialogueInteractionConfig config;

        [Header("Radio Cutscene")] 
        [SerializeField] private PlayableDirector radioTimeline;

        private Transform _playerCamera;

        protected override void Awake()
        {
            base.Awake();

            if (radioTimeline != null)
            {
                radioTimeline.Stop();
                radioTimeline.time = 0;
                radioTimeline.Evaluate();
            }

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null)
                    _playerCamera = cam.transform;
            }
        }

        private void Update()
        {
            if (interactionCanvas != null && interactionCanvas.gameObject.activeSelf && _playerCamera != null)
            {
                interactionCanvas.transform.LookAt(_playerCamera);
                interactionCanvas.transform.Rotate(0f, 180f, 0f);
            }
        }

        public override void Interact(IInteractor interactor)
        {
            // bij deze methode roepen we de timeline aan 
            radioTimeline.Play();

            if (dialogueLoader.gameObject.activeSelf ||
                config.dialogueNodes == null ||
                config.dialogueNodes.Count == 0)
                return;

            dialogueLoader.gameObject.SetActive(true);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var movement = player.GetComponent<MovementController>();
                var cameraCtrl = player.GetComponentInChildren<CameraController>();

                if (config.pausePlayerMovement)
                    movement?.PauseMovement();

                if (config.pauseCameraMovement)
                    cameraCtrl?.PauseCameraMovement();
            }

            if (config.showCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            dialogueLoader.StartDialogue(config);

            if (interactionUI != null)
                interactionUI.SetActive(false);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            bool canInteract = dialogueLoader != null &&
                               !dialogueLoader.gameObject.activeSelf &&
                               config.dialogueNodes is { Count: > 0 };

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(canInteract);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
    }
}