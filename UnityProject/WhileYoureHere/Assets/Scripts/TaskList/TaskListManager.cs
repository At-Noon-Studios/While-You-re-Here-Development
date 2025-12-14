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

        private void OnEnable()
        {
            Build();
        }

        public void Build()
        {
            foreach (Transform child in taskListCanvas.transform)
                Destroy(child.gameObject);

            for (int i = taskLoader.tasks.Count - 1; i >= 0; i--)
            {
                var task = taskLoader.tasks[i];
                var toggle = Instantiate(taskTogglePrefab, taskListCanvas.transform)
                    .GetComponent<Toggle>();

                toggle.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = task.taskName;
                toggle.isOn = task.isCompleted;

                string id = task.taskID;
                toggle.onValueChanged.AddListener(v =>
                    taskLoader.SetTaskCompleted(id, v)
                );
            }
        }
    }
}