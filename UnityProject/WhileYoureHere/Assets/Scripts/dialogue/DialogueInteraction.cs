using UnityEngine;
using Interactable;
using player_controls;
using ScriptableObjects.dialogue;

namespace dialogue
{
    public class DialogueInteraction : InteractableBehaviour
    {
        [SerializeField] private DialogueLoader dialogueLoader;
        [SerializeField] private DialogueInteractionConfig config;
        [SerializeField] private GameObject interactionUI;

        public override void Interact(IInteractor interactor)
        {
            if (dialogueLoader.gameObject.activeSelf || config.dialogueNodes == null || config.dialogueNodes.Count == 0)
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
            
            dialogueLoader.StartDialogue(config.dialogueNodes);

            interactionUI.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor)
        {
            return "Talk";
        }
    }
}