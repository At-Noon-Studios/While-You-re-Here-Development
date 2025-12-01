using chore;
using UnityEngine;

namespace ScriptableObjects.chores
{
    public abstract class SoChoreComponent : ScriptableObject
    {
        [Header("Component Settings")] public string componentName;
        public ChoreComponent.ChoreComponentType choreType;
        [TextArea(2, 10)] public string description;
    }
}
