using System;
using UnityEngine;

namespace chore
{
    public abstract class ChoreComponent
    {
        public bool IsActive { get; private set; }
        
        public enum ChoreComponentType
        {
            WateringCanPickedUp,
            WateringCanFilled,
            PlantWatered,
            KettleFilled,
            WaterBoiled,
            TeabagAdded,
            CupFilled
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
            IsActive = true;
            Debug.Log($"Component {ComponentName} has been enabled.");
        }

        public virtual void MarkCompleted()
        {
            IsActive = false;
        }
    }
}
