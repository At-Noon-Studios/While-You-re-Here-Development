using System;
using dialogue;
using EventChannels;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueLoader dialogueLoader;

        [Header("UI Settings")]
        [SerializeField] private GameObject dialogueUI;

        [Header("Interaction Settings")]
        [SerializeField] private float interactDistance;
        
        [Header("Player Settings")]
        [SerializeField] private Transform player;
        [SerializeField] private EventChannel interactEvent;

        [Obsolete("Obsolete")]
        private void OnEnable()
        {
            interactEvent.OnRaise += TryTriggerDialogue;
        }

        [Obsolete("Obsolete")]
        private void OnDisable()
        {
            interactEvent.OnRaise -= TryTriggerDialogue;
        }

        [Obsolete("Obsolete")]
        private void TryTriggerDialogue()
        {
            if (Vector3.Distance(player.position, transform.position) <= interactDistance)
            {
                var interactPrompt = GetComponent<InteractPrompt>();
                interactPrompt?.BeginInteraction();

                dialogueUI?.SetActive(true);
                dialogueLoader?.StartDialogue();

                var movementController = player.GetComponent<MovementController>();
                movementController?.PauseMovement();

                var cameraController = player.GetComponentInChildren<CameraController>();
                cameraController?.PauseCameraMovement();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}