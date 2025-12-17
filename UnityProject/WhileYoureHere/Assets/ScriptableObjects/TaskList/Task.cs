using UnityEngine;

namespace ScriptableObjects.TaskList
{
    [CreateAssetMenu(fileName = "Task", menuName = "ScriptableObjects/Task")]
    public class Task : ScriptableObject
    {
        public int taskID;
        public string taskName;

        [Header("Sprites")]
        public Sprite uncompletedSprite;
        public Sprite completedSprite;

        [HideInInspector]
        public bool isCompleted;
    }
}
