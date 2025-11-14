using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueLoader dialogueLoader;
    public float interactDistance = 2f;
    public GameObject dialogueUI;

    [Header("Player Reference")]
    public Transform player;
    public VoidEventChannel interactEvent;

    private void OnEnable()
    {
        interactEvent.OnRaise += TryTriggerDialogue;
    }

    private void OnDisable()
    {
        interactEvent.OnRaise -= TryTriggerDialogue;
    }

    private void TryTriggerDialogue()
    {
        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            GetComponent<InteractPrompt>()?.BeginInteraction();

            dialogueUI?.SetActive(true);
            dialogueLoader?.StartDialogue();

            player.GetComponent<MovementController>()?.PauseMovement();
            player.GetComponentInChildren<CameraController>().PauseCameraMovement();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}