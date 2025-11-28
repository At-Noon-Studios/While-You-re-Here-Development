using Interactable;
using UnityEngine;
using player_controls;
using UI;

namespace dialogue
{
    public class DialogueInteraction : InteractableBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueLoader dialogueLoader;

        [Header("UI Settings")]
        [SerializeField] private GameObject dialogueUI;

        protected new void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
            if (!dialogueLoader.gameObject.activeSelf)
            {
                dialogueLoader.gameObject.SetActive(true);
            }

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var movementController = player.GetComponent<MovementController>();
                movementController?.PauseMovement();

                var cameraController = player.GetComponentInChildren<CameraController>();
                cameraController?.PauseCameraMovement();
            }

            if (dialogueLoader != null)
            {
                dialogueLoader.StartDialogue();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        protected override string InteractionText()
        {
            return "Talk";
        }
    }
}