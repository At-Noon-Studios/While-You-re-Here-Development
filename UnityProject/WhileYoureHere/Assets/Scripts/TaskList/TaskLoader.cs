using System.Collections.Generic;
using ScriptableObjects.TaskList;
using UnityEngine;

namespace TaskList
{
    public class TaskLoader : MonoBehaviour
    {
        [SerializeField] public List<Task> tasks;

        private readonly Dictionary<string, Task> _taskDictionary = new();

        private void Awake()
        {
            foreach (var task in tasks)
                _taskDictionary[task.taskID] = task;
        }

        public void SetTaskCompleted(string id, bool value)
        {
            if (_taskDictionary.TryGetValue(id, out var task))
                task.isCompleted = value;
        }
    }
}