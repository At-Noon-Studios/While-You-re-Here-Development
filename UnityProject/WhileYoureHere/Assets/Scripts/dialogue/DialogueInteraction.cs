using UnityEngine;
using Interactable;
using player_controls;

namespace dialogue
{
    public class DialogueInteraction : InteractableBehaviour
    {
        [SerializeField] private DialogueLoader dialogueLoader;
        [SerializeField] private GameObject interactionText;

        public override void Interact()
        {
            if (dialogueLoader.gameObject.activeSelf)
                return;

            dialogueLoader.gameObject.SetActive(true);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.GetComponent<MovementController>()?.PauseMovement();
                player.GetComponentInChildren<CameraController>()?.PauseCameraMovement();
            }

            dialogueLoader.StartDialogue();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            interactionText.gameObject.SetActive(false);
        }

        protected override string InteractionText()
        {
            return "Talk";
        }
    }
}