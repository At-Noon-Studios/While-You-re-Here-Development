using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScriptableObjects.TaskList;

namespace TaskList
{
    public class TaskListManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TaskLoader taskLoader;
        [SerializeField] private GameObject taskImagePrefab;
        [SerializeField] private Transform taskListContainer;

        private readonly Dictionary<int, Image> _taskImages = new();

        private void Awake()
        {
            if(taskLoader == null)
                taskLoader = FindObjectOfType<TaskLoader>();

            if(taskLoader == null)
                Debug.LogError("TaskLoader not found in scene!");
        }

        private void Start()
        {
            ResetTasksOnStart();
            Build();
        }


        public void Build()
        {
            _taskImages.Clear();

            foreach (Transform child in taskListContainer)
                Destroy(child.gameObject);

            foreach (var task in taskLoader.tasks)
            {
                var go = Instantiate(taskImagePrefab, taskListContainer);
                var image = go.GetComponent<Image>();

                if (image == null)
                {
                    Debug.LogError("Task prefab must have an Image component");
                    continue;
                }

                image.sprite = task.isCompleted ? task.completedSprite : task.uncompletedSprite;

                go.transform.SetAsFirstSibling();

                _taskImages[task.taskID] = image;
            }
        }

        public void TriggerCheckmark(int taskID)
        {
            taskLoader.SetTaskCompleted(taskID);

            var task = taskLoader.GetTaskByID(taskID);
            if(task == null)
            {
                Debug.LogWarning($"No task found with ID {taskID}");
                return;
            }

            if (_taskImages.TryGetValue(taskID, out var image))
            {
                image.sprite = task.completedSprite;
                Debug.Log($"Task {taskID} completed -> sprite updated");
            }
            else
            {
                Debug.LogWarning($"No image found for taskID {taskID}");
            }
        }

        private void ResetTasksOnStart()
        {
            foreach (var task in taskLoader.tasks)
            {
                task.isCompleted = false;
            }
        }
    }
}
