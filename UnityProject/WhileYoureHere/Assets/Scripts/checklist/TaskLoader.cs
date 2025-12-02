using System.Collections.Generic;
using ScriptableObjects.CheckList;
using UnityEngine;

namespace checklist
{
    public class TaskLoader : MonoBehaviour
    {
        [Header("Assign the Tasks here")]
        public List<Task> tasks;

        private Dictionary<string, Task> _taskDictionary = new Dictionary<string, Task>();

        private void Awake()
        {
            // Zet alle tasks in een dictionary voor makkelijk ophalen
            foreach (var t in tasks)
            {
                if (!_taskDictionary.ContainsKey(t.taskID))
                {
                    _taskDictionary.Add(t.taskID, t);
                }
                else
                {
                    Debug.LogWarning($"Duplicate Task ID: {t.taskID}");
                }
            }
        }

        // Haal een task op basis van ID
        public Task GetTaskByID(string taskID)
        {
            if (_taskDictionary.TryGetValue(taskID, out var task))
                return task;
            return null;
        }

        // Markeer task als voltooid
        public void CompleteTask(string taskID)
        {
            var task = GetTaskByID(taskID);
            if (task != null)
            {
                task.isCompleted = true;
                Debug.Log($"Task completed: {task.taskName}");
            }
        }

        // Optioneel: return alle incomplete tasks
        public List<Task> GetIncompleteTasks()
        {
            List<Task> incomplete = new List<Task>();
            foreach (var t in _taskDictionary.Values)
            {
                if (!t.isCompleted) incomplete.Add(t);
            }
            return incomplete;
        }
    }
}