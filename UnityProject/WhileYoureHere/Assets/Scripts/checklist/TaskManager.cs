using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace checklist
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField] private TaskLoader taskLoader;
        [SerializeField] private GameObject togglePrefab;
        [SerializeField] private Transform toggleContainer;

        private readonly Dictionary<string, Toggle> taskToggles = new();

        private void Start()
        {
            foreach (var task in taskLoader.tasks)
            {
                var toggle = Instantiate(togglePrefab, toggleContainer).GetComponent<Toggle>();
                toggle.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = task.taskName;
                toggle.isOn = task.isCompleted;

                string taskID = task.taskID;
                toggle.onValueChanged.AddListener(value =>
                {
                    if (value) taskLoader.CompleteTask(taskID);
                    else taskLoader.GetTaskByID(taskID).isCompleted = false;
                });

                taskToggles[taskID] = toggle;
            }
        }
    }
}