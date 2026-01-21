using UnityEngine;
using player_controls;
using PlayerControls;
using ScriptableObjects.dialogue;

namespace dialogue
{
    public class DialogueProximityTrigger : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueLoader dialogueLoader;
        [SerializeField] private DialogueInteractionConfig config;

        [Header("Trigger Settings")]
        [SerializeField] private float triggerRange = 3f;

        [Header("Player Detection")]
        [SerializeField] private Transform playerTransform;

        private bool _wasInRange;
        private bool _hasTriggered;
        
        private void Awake()
        {
            FindPlayer();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                FindPlayer();
            }
        }

        private void FindPlayer()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
        }

        private void Update()
        {
            if (_hasTriggered) return;

            if (playerTransform == null)
            {
                FindPlayer();
                return;
            }

            if (dialogueLoader == null || config == null) return;
            if (config.dialogueNodes == null || config.dialogueNodes.Count == 0) return;

            var inRange = IsPlayerInTriggerZone();
            var enteredThisFrame = inRange && !_wasInRange;

            if (enteredThisFrame)
            {
                TriggerDialogue();
            }

            _wasInRange = inRange;
        }

        private bool IsPlayerInTriggerZone()
        {
            var distance = Vector3.Distance(playerTransform.position, transform.position);
            return distance <= triggerRange;
        }

        private void TriggerDialogue()
        {
            if (_hasTriggered) return;

            _hasTriggered = true;

            dialogueLoader.gameObject.SetActive(true);

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
        }
    }
}
