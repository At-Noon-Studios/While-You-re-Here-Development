using UnityEngine;
using ScriptableObjects.Events;
using UnityEngine.InputSystem;

namespace TaskList
{
    public class TaskListInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TaskListUI taskListUI;
        [SerializeField] private EventChannel taskListEventChannel;

        public void OnTaskList()
        {
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