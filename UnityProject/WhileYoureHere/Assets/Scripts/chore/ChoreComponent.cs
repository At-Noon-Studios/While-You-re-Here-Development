using System;
using UnityEngine;

namespace chore
{
    public abstract class ChoreComponent
    {
        public enum ChoreComponentType
        {
            EnemyKilled,
            ItemCollected
        }

        public event Action<ChoreComponent> OnComponentCompleted;

        protected void TriggerComponentCompleted(ChoreComponent choreComponent)
        {
            OnComponentCompleted?.Invoke(choreComponent);
        }

        public string ComponentName;
        public string ComponentDescription;
        public ChoreComponentType ComponentType;

        protected ChoreComponent(string name, string description)
        {
            ComponentName = name;
            ComponentDescription = description;
        }

        public virtual void EnableComponent()
        {
            Debug.Log($"{ComponentName} has been enabled.");
        }

        public virtual void MarkCompleted()
        {
            Debug.Log($"{ComponentName} has been completed.");
        }
    }
}
