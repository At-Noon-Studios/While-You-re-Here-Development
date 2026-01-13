using dialogue;
using ScriptableObjects.chopping_logs;
using ScriptableObjects.dialogue;
using UnityEngine;

namespace chopping_logs
{
    public class StumpDialogue : MonoBehaviour
    {
        [SerializeField] private StumpDialogueData stumpDialogueData;
        [SerializeField] private DialogueInteractionConfig dialogueConfig;
        private DialogueManager _dialogueManager;
        private Vector3 _playerPosition;
    
        private void Start()
        {
            _dialogueManager = DialogueManager.Instance;
            if (_dialogueManager == null) Debug.LogError("Unable to obtain dialogue manager instance");
        
            _playerPosition = GameObject.FindWithTag("Player").transform.position;
        }
    
        private void Update()
        {
            if (Vector2.Distance(_playerPosition, transform.position) > stumpDialogueData.Range)
            {
                _dialogueManager.StartDialogue(dialogueConfig);
            }
        }
    }
}
