using System.Collections.Generic;
using ScriptableObjects.TodoList;
using UnityEngine;

namespace checklist
{
    public class TaskLoader : MonoBehaviour
    {
        [SerializeField] public List<Task> tasks;
        private readonly Dictionary<string, Task> _taskDictionary = new();

        private void Awake()
        {
            foreach (var t in tasks) _taskDictionary[t.taskID] = t;
        }

        public Task GetTaskByID(string taskID) => _taskDictionary.GetValueOrDefault(taskID);

        public void CompleteTask(string taskID) => _taskDictionary[taskID].isCompleted = true;
    }
}