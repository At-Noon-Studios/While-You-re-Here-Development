using UnityEngine;

namespace TaskList
{
    public class TaskListInputHandler : MonoBehaviour
    {
        [SerializeField] private TaskListUI taskListUI;

        public void OnTaskList()
        {
            taskListUI.ToggleTaskList();
        }
    }
}