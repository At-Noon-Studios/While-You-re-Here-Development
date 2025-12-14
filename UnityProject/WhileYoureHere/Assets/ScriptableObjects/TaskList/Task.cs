using UnityEngine;

namespace ScriptableObjects.TaskList
{
    [CreateAssetMenu(fileName = "Task", menuName = "ScriptableObjects/Task")]
    public class Task : ScriptableObject
    {
        public string taskID;
        public string taskName;
        public bool isCompleted;
    }
}