using System;
using UnityEngine;

namespace chore
{
    public abstract class ChoreComponent
    {
        public enum ChoreComponentType
        {
            EnemyKilled,
            ItemCollected,
            DoorOpened
        }
        
        public bool IsActive { get; private set; }

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
            IsActive = true;
            Debug.Log($"Component {ComponentName} has been enabled.");
        }

        public virtual void MarkCompleted()
        {
            IsActive = false;
        }
    }
}
