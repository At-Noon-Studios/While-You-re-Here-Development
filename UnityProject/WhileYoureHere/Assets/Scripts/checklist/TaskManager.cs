using System.Collections.Generic;
using checklist;
using ScriptableObjects.CheckList;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public TaskLoader taskLoader;          // Je TaskLoader met alle ScriptableObject tasks
    public GameObject togglePrefab;        // Prefab met een Toggle component
    public Transform toggleContainer;      // Parent waar de toggles in komen

    private Dictionary<string, Toggle> _taskToggles = new Dictionary<string, Toggle>();

    private void Start()
    {
        if (taskLoader == null || togglePrefab == null || toggleContainer == null) return;

        foreach (var task in taskLoader.tasks)
        {
            GameObject toggleGO = Instantiate(togglePrefab, toggleContainer);
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            if (toggle == null) continue;

            // Zet de tekst van de toggle
            Text label = toggle.GetComponentInChildren<Text>();
            if (label != null) label.text = task.taskName;

            // Zet de toggle aan/uit op basis van isCompleted
            toggle.isOn = task.isCompleted;

            string taskID = task.taskID; // nodig voor de lambda

            toggle.onValueChanged.AddListener((value) =>
            {
                if (value) taskLoader.CompleteTask(taskID);
                else
                {
                    Task t = taskLoader.GetTaskByID(taskID);
                    if (t != null) t.isCompleted = false;
                }
            });

            _taskToggles.Add(taskID, toggle);
        }
    }

    // Optioneel: update de toggles als tasks elders veranderd worden
    public void RefreshToggles()
    {
        foreach (var kvp in _taskToggles)
        {
            Task t = taskLoader.GetTaskByID(kvp.Key);
            if (t != null)
                kvp.Value.isOn = t.isCompleted;
        }
    }
}