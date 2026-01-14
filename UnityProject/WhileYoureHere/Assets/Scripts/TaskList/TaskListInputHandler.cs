using Interactable;
using UnityEngine;
using ScriptableObjects.Events;

namespace TaskList
{
    public class TaskListInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TaskListUI taskListUI;
        [SerializeField] private TaskListSound taskListSound;
        [SerializeField] private EventChannel taskListEventChannel;

        private PlayerInteractionController _player;

        private void Awake()
        {
            _player = FindAnyObjectByType<PlayerInteractionController>();
        }

        public void OnTaskList()
        {
            
            if (_player != null && _player.IsTableMode)
                return;
            
            taskListEventChannel?.Raise();
        }

        private void OnEnable()
        {
            if (taskListEventChannel != null)
                taskListEventChannel.OnRaise += HandleTaskListToggle;
        }

        private void OnDisable()
        {
            if (taskListEventChannel != null)
                taskListEventChannel.OnRaise -= HandleTaskListToggle;
        }

        private void HandleTaskListToggle()
        {
            if (taskListUI != null)
                taskListUI.ToggleTaskList();
        }
    }
}