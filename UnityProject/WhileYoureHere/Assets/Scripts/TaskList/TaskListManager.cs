using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TaskList
{
    public class TaskListManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TaskLoader taskLoader;
        [SerializeField] private GameObject taskTogglePrefab;
        [SerializeField] private Canvas taskListCanvas;

        private Dictionary<int, Toggle> _taskToggles = new Dictionary<int, Toggle>();

        private void OnEnable()
        {
            Build();
        }

        private void OnDisable()
        {
            ResetAllToggles();
        }

        public void Build()
        {
            _taskToggles.Clear();

            foreach (Transform child in taskListCanvas.transform)
                Destroy(child.gameObject);

            for (int i = taskLoader.tasks.Count - 1; i >= 0; i--)
            {
                var task = taskLoader.tasks[i];
                var toggle = Instantiate(taskTogglePrefab, taskListCanvas.transform)
                    .GetComponent<Toggle>();

                toggle.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = task.taskName;
                toggle.isOn = task.isCompleted;

                int id = task.taskID;
                toggle.onValueChanged.AddListener(v =>
                    taskLoader.SetTaskCompleted(id, v)
                );

                _taskToggles[id] = toggle;
            }
        }

        public void TriggerCheckmark(int choreID)
        {
            if (_taskToggles.TryGetValue(choreID, out var toggle))
            {
                toggle.isOn = true;
                taskLoader.SetTaskCompleted(choreID, true);
                Debug.Log($"Chore {choreID} completed -> Task {choreID} checked");
            }
            else
            {
                Debug.LogWarning($"No toggle found for choreID/taskID {choreID}");
            }
        }

        private void ResetAllToggles()
        {
            foreach (var toggle in _taskToggles.Values)
            {
                if (toggle != null)
                    toggle.isOn = false;
            }
        }
    }
}
